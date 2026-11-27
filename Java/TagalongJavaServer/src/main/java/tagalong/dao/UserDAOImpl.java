package tagalong.dao;

import tagalong.grpc.User;
import tagalong.grpc.UserCredentials;
import tagalong.database.DatabaseConnection;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.Optional;

public class UserDAOImpl implements UserDAO
{
    @Override
    public User create(String email, String name, String passwordHash) throws SQLException
    {
        String sql = "INSERT INTO users (email, name, password_hash) "
                + "VALUES (?, ?, ?) RETURNING id";

        try (Connection connection = DatabaseConnection.getConnection();
             PreparedStatement statement = connection.prepareStatement(sql))
        {
            statement.setString(1, email);
            statement.setString(2, name);
            statement.setString(3, passwordHash);

            try (ResultSet rs = statement.executeQuery())
            {
                rs.next();
                return User.newBuilder()
                        .setId(rs.getLong("id"))
                        .setEmail(email)
                        .setName(name)
                        .build();
            }
        }
    }

    @Override
    public Optional<User> findById(long id) throws SQLException
    {
        String sql = "SELECT id, email, name FROM users WHERE id = ?";

        try (Connection connection = DatabaseConnection.getConnection();
             PreparedStatement statement = connection.prepareStatement(sql))
        {
            statement.setLong(1, id);

            try (ResultSet rs = statement.executeQuery())
            {
                if (rs.next())
                {
                    return Optional.of(toUser(rs));
                }
                return Optional.empty();
            }
        }
    }

    @Override
    public Optional<UserCredentials> findCredentialsByEmail(String email) throws SQLException
    {
        String sql = "SELECT id, email, name, password_hash FROM users WHERE email = ?";

        try (Connection connection = DatabaseConnection.getConnection();
             PreparedStatement statement = connection.prepareStatement(sql))
        {
            statement.setString(1, email);

            try (ResultSet rs = statement.executeQuery())
            {
                if (rs.next())
                {
                    return Optional.of(UserCredentials.newBuilder()
                            .setUser(toUser(rs))
                            .setPasswordHash(rs.getString("password_hash"))
                            .build());
                }
                return Optional.empty();
            }
        }
    }

    private User toUser(ResultSet rs) throws SQLException
    {
        return User.newBuilder()
                .setId(rs.getLong("id"))
                .setEmail(rs.getString("email"))
                .setName(rs.getString("name"))
                .build();
    }
}