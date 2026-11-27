package tagalong.dao;

import tagalong.grpc.Invite;

import java.sql.SQLException;
import java.time.Instant;
import java.util.Optional;

public interface InviteDAO {
    Invite create(long tripId, long createdBy, String token, Instant expiresAt) throws SQLException;

        Optional<Invite> findByToken(String token) throws  SQLException;
}
