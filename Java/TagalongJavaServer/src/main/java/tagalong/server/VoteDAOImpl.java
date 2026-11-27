package tagalong.dao;

import tagalong.database.DatabaseConnection;
import tagalong.grpc.ActivityVotes;
import tagalong.grpc.VoteInfo;
import tagalong.grpc.VoteValue;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;

public class VoteDAOImpl implements VoteDAO
{
    @Override
    public void setVote(long activityId, long userId, VoteValue value) throws SQLException
    {
        String sql = "INSERT INTO votes (activity_id, user_id, value) VALUES (?, ?, ?) "
                + "ON CONFLICT (activity_id, user_id) DO UPDATE SET value = EXCLUDED.value";

        try (Connection connection = DatabaseConnection.getConnection();
             PreparedStatement statement = connection.prepareStatement(sql))
        {
            statement.setLong(1, activityId);
            statement.setLong(2, userId);
            statement.setShort(3, (short) (value == VoteValue.VOTE_VALUE_UP ? 1 : -1));
            statement.executeUpdate();
        }
    }

    @Override
    public void removeVote(long activityId, long userId) throws SQLException
    {
        String sql = "DELETE FROM votes WHERE activity_id = ? AND user_id = ?";

        try (Connection connection = DatabaseConnection.getConnection();
             PreparedStatement statement = connection.prepareStatement(sql))
        {
            statement.setLong(1, activityId);
            statement.setLong(2, userId);
            statement.executeUpdate();
        }
    }

    @Override
    public ActivityVotes getVotes(long activityId, long currentUserId) throws SQLException
    {
        String sql = "SELECT v.user_id, u.name, v.value FROM votes v "
                + "JOIN users u ON u.id = v.user_id WHERE v.activity_id = ? ORDER BY v.id";

        try (Connection connection = DatabaseConnection.getConnection();
             PreparedStatement statement = connection.prepareStatement(sql))
        {
            statement.setLong(1, activityId);

            ActivityVotes.Builder result = ActivityVotes.newBuilder().setActivityId(activityId);
            int upvotes = 0;
            int downvotes = 0;

            try (ResultSet rs = statement.executeQuery())
            {
                while (rs.next())
                {
                    long userId = rs.getLong("user_id");
                    VoteValue value = rs.getShort("value") == 1
                            ? VoteValue.VOTE_VALUE_UP
                            : VoteValue.VOTE_VALUE_DOWN;

                    if (value == VoteValue.VOTE_VALUE_UP)
                    {
                        upvotes++;
                    }
                    else
                    {
                        downvotes++;
                    }

                    if (userId == currentUserId)
                    {
                        result.setMyVote(value);
                    }

                    result.addVotes(VoteInfo.newBuilder()
                            .setUserId(userId)
                            .setUserName(rs.getString("name"))
                            .setValue(value));
                }
            }

            return result
                    .setUpvotes(upvotes)
                    .setDownvotes(downvotes)
                    .setScore(upvotes - downvotes)
                    .build();
        }
    }
}
