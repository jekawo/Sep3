package tagalong.dao;

import com.google.protobuf.Timestamp;
import tagalong.database.DatabaseConnection;
import tagalong.grpc.Activity;

import java.math.BigDecimal;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Types;
import java.time.Instant;
import java.util.ArrayList;
import java.util.List;
import java.util.Optional;

public class ActivityDAOImpl implements ActivityDAO
{
    private static final String COLUMNS =
            "id, trip_id, title, description, price, location, link, created_by, created_at";

    @Override
    public Activity create(long userId, long tripId, String title, String description,
                           Double price, String location, String link) throws SQLException
    {
        String sql = "INSERT INTO activities (trip_id, title, description, price, location, "
                + "link, created_by) VALUES (?, ?, ?, ?, ?, ?, ?) RETURNING " + COLUMNS;

        try (Connection connection = DatabaseConnection.getConnection();
             PreparedStatement statement = connection.prepareStatement(sql))
        {
            statement.setLong(1, tripId);
            statement.setString(2, title);
            statement.setString(3, description);
            if (price == null)
            {
                statement.setNull(4, Types.NUMERIC);
            }
            else
            {
                statement.setBigDecimal(4, BigDecimal.valueOf(price));
            }
            statement.setString(5, location);
            statement.setString(6, link);
            statement.setLong(7, userId);

            try (ResultSet rs = statement.executeQuery())
            {
                rs.next();
                return toActivity(rs);
            }
        }
    }

    @Override
    public Optional<Activity> findById(long activityId) throws SQLException
    {
        String sql = "SELECT " + COLUMNS + " FROM activities WHERE id = ?";

        try (Connection connection = DatabaseConnection.getConnection();
             PreparedStatement statement = connection.prepareStatement(sql))
        {
            statement.setLong(1, activityId);

            try (ResultSet rs = statement.executeQuery())
            {
                if (rs.next())
                {
                    return Optional.of(toActivity(rs));
                }
                return Optional.empty();
            }
        }
    }

    @Override
    public List<Activity> findByTrip(long tripId) throws SQLException
    {
        String sql = "SELECT " + COLUMNS + " FROM activities WHERE trip_id = ? ORDER BY created_at, id";

        try (Connection connection = DatabaseConnection.getConnection();
             PreparedStatement statement = connection.prepareStatement(sql))
        {
            statement.setLong(1, tripId);

            try (ResultSet rs = statement.executeQuery())
            {
                List<Activity> activities = new ArrayList<>();
                while (rs.next())
                {
                    activities.add(toActivity(rs));
                }
                return activities;
            }
        }
    }

    private Activity toActivity(ResultSet rs) throws SQLException
    {
        Instant createdAt = rs.getTimestamp("created_at").toInstant();

        Activity.Builder builder = Activity.newBuilder()
                .setId(rs.getLong("id"))
                .setTripId(rs.getLong("trip_id"))
                .setTitle(rs.getString("title"))
                .setDescription(nullToEmpty(rs.getString("description")))
                .setLocation(nullToEmpty(rs.getString("location")))
                .setLink(nullToEmpty(rs.getString("link")))
                .setCreatedBy(rs.getLong("created_by"))
                .setCreatedAt(Timestamp.newBuilder()
                        .setSeconds(createdAt.getEpochSecond())
                        .setNanos(createdAt.getNano())
                        .build());

        BigDecimal price = rs.getBigDecimal("price");
        if (price != null)
        {
            builder.setPrice(price.doubleValue());
        }
        return builder.build();
    }

    private String nullToEmpty(String value)
    {
        return value == null ? "" : value;
    }
}