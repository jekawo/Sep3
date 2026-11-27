package tagalong.dao;

import tagalong.grpc.User;
import tagalong.grpc.UserCredentials;

import java.sql.SQLException;
import java.util.Optional;

public interface UserDAO {
    User create(String email, String name, String passwordHash) throws SQLException;
    Optional<User> findById(long id) throws SQLException;

    Optional<UserCredentials> findCredentialsByEmail(String email) throws SQLException;

}
