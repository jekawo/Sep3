package tagalong.dao;

import com.google.protobuf.Timestamp;
import tagalong.database.DatabaseConnection;
import tagalong.grpc.Invite;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.time.Instant;
import java.util.Optional;

public class InviteDAOImpl implements InviteDAO
{
    @Override
    public Invite create(long tripId, long createdBy, String token, Instant expiresAt)
            throws SQLException
    {
        String sql = "INSERT INTO trip_invites (trip_id, token, expires_at, created_by) "
                + "VALUES (?, ?, ?, ?) RETURNING id";

        try (Connection connection = DatabaseConnection.getConnection();
             PreparedStatement statement = connection.prepareStatement(sql))
        {
            statement.setLong(1, tripId);
            statement.setString(2, token);
            statement.setTimestamp(3, java.sql.Timestamp.from(expiresAt));
            statement.setLong(4, createdBy);

            try (ResultSet rs = statement.executeQuery())
            {
                rs.next();
                return Invite.newBuilder()
                        .setId(rs.getLong("id"))
                        .setTripId(tripId)
                        .setToken(token)
                        .setExpiresAt(toProtoTimestamp(expiresAt))
                        .build();
            }
        }
    }

    @Override
    public Optional<Invite> findByToken(String token) throws SQLException
    {
        String sql = "SELECT id, trip_id, token, expires_at FROM trip_invites WHERE token = ?";

        try (Connection connection = DatabaseConnection.getConnection();
             PreparedStatement statement = connection.prepareStatement(sql))
        {
            statement.setString(1, token);

            try (ResultSet rs = statement.executeQuery())
            {
                if (rs.next())
                {
                    return Optional.of(Invite.newBuilder()
                            .setId(rs.getLong("id"))
                            .setTripId(rs.getLong("trip_id"))
                            .setToken(rs.getString("token"))
                            .setExpiresAt(toProtoTimestamp(rs.getTimestamp("expires_at").toInstant()))
                            .build());
                }
                return Optional.empty();
            }
        }
    }

    private Timestamp toProtoTimestamp(Instant instant)
    {
        return Timestamp.newBuilder()
                .setSeconds(instant.getEpochSecond())
                .setNanos(instant.getNano())
                .build();
    }
}