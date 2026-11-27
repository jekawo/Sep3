package tagalong.dao;

import tagalong.grpc.ActivityVotes;
import tagalong.grpc.VoteValue;

import java.sql.SQLException;

public interface VoteDAO {
    void setVote(long activityId, long userId, VoteValue value) throws SQLException;

    void removeVote(long activityId, long userId) throws SQLException;

    ActivityVotes getVotes(long activityId, long currentUserId) throws SQLException;
}
