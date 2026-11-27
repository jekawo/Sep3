package tagalong.dao;

import com.google.protobuf.Timestamp;
import tagalong.grpc.Member;
import tagalong.grpc.Trip;
import tagalong.database.DatabaseConnection;

import java.sql.Connection;
import java.sql.Date;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.time.Instant;
import java.time.LocalDate;
import java.util.ArrayList;
import java.util.List;
import java.util.Optional;

public class TripDAOImpl implements TripDAO
{
    @Override
    public Trip create(long userId, String title, String destination,
                       String startDate, String endDate, String description) throws SQLException
    {
        String tripSql = "INSERT INTO trips (title, destination, start_date, end_date, "
                + "description, created_by) VALUES (?, ?, ?, ?, ?, ?) RETURNING id";
        String memberSql = "INSERT INTO trip_members (trip_id, user_id) VALUES (?, ?)";

        try (Connection connection = DatabaseConnection.getConnection())
        {
            connection.setAutoCommit(false);
            try
            {
                long tripId;
                try (PreparedStatement statement = connection.prepareStatement(tripSql))
                {
                    statement.setString(1, title);
                    statement.setString(2, destination);
                    statement.setDate(3, toDate(startDate));
                    statement.setDate(4, toDate(endDate));
                    statement.setString(5, description);
                    statement.setLong(6, userId);

                    try (ResultSet rs = statement.executeQuery())
                    {
                        rs.next();
                        tripId = rs.getLong("id");
                    }
                }

                try (PreparedStatement statement = connection.prepareStatement(memberSql))
                {
                    statement.setLong(1, tripId);
                    statement.setLong(2, userId);
                    statement.executeUpdate();
                }

                connection.commit();

                return Trip.newBuilder()
                        .setId(tripId)
                        .setTitle(title)
                        .setDestination(nullToEmpty(destination))
                        .setStartDate(nullToEmpty(startDate))
                        .setEndDate(nullToEmpty(endDate))
                        .setDescription(nullToEmpty(description))
                        .setCreatedBy(userId)
                        .build();
            }
            catch (SQLException e)
            {
                connection.rollback();
                throw e;
            }
        }
    }

    @Override
    public Optional<Trip> findById(long tripId) throws SQLException
    {
        String sql = "SELECT id, title, destination, start_date, end_date, description, "
                + "created_by FROM trips WHERE id = ?";

        try (Connection connection = DatabaseConnection.getConnection();
             PreparedStatement statement = connection.prepareStatement(sql))
        {
            statement.setLong(1, tripId);

            try (ResultSet rs = statement.executeQuery())
            {
                if (rs.next())
                {
                    return Optional.of(toTrip(rs));
                }
                return Optional.empty();
            }
        }
    }

    @Override
    public List<Trip> findByUser(long userId) throws SQLException
    {
        String sql = "SELECT t.id, t.title, t.destination, t.start_date, t.end_date, "
                + "t.description, t.created_by FROM trips t "
                + "JOIN trip_members m ON m.trip_id = t.id "
                + "WHERE m.user_id = ? ORDER BY t.start_date NULLS LAST, t.id";

        try (Connection connection = DatabaseConnection.getConnection();
             PreparedStatement statement = connection.prepareStatement(sql))
        {
            statement.setLong(1, userId);

            try (ResultSet rs = statement.executeQuery())
            {
                List<Trip> trips = new ArrayList<>();
                while (rs.next())
                {
                    trips.add(toTrip(rs));
                }
                return trips;
            }
        }
    }

    @Override
    public List<Member> findMembers(long tripId) throws SQLException
    {
        String sql = "SELECT m.user_id, u.name, m.joined_at FROM trip_members m "
                + "JOIN users u ON u.id = m.user_id WHERE m.trip_id = ? ORDER BY m.joined_at";

        try (Connection connection = DatabaseConnection.getConnection();
             PreparedStatement statement = connection.prepareStatement(sql))
        {
            statement.setLong(1, tripId);

            try (ResultSet rs = statement.executeQuery())
            {
                List<Member> members = new ArrayList<>();
                while (rs.next())
                {
                    Instant joinedAt = rs.getTimestamp("joined_at").toInstant();
                    members.add(Member.newBuilder()
                            .setUserId(rs.getLong("user_id"))
                            .setName(rs.getString("name"))
                            .setJoinedAt(Timestamp.newBuilder()
                                    .setSeconds(joinedAt.getEpochSecond())
                                    .setNanos(joinedAt.getNano())
                                    .build())
                            .build());
                }
                return members;
            }
        }
    }

    @Override
    public boolean isMember(long tripId, long userId) throws SQLException
    {
        String sql = "SELECT 1 FROM trip_members WHERE trip_id = ? AND user_id = ?";

        try (Connection connection = DatabaseConnection.getConnection();
             PreparedStatement statement = connection.prepareStatement(sql))
        {
            statement.setLong(1, tripId);
            statement.setLong(2, userId);

            try (ResultSet rs = statement.executeQuery())
            {
                return rs.next();
            }
        }
    }

    @Override
    public void addMember(long tripId, long userId) throws SQLException
    {
        String sql = "INSERT INTO trip_members (trip_id, user_id) VALUES (?, ?) "
                + "ON CONFLICT DO NOTHING";

        try (Connection connection = DatabaseConnection.getConnection();
             PreparedStatement statement = connection.prepareStatement(sql))
        {
            statement.setLong(1, tripId);
            statement.setLong(2, userId);
            statement.executeUpdate();
        }
    }

    private Trip toTrip(ResultSet rs) throws SQLException
    {
        Date start = rs.getDate("start_date");
        Date end = rs.getDate("end_date");

        return Trip.newBuilder()
                .setId(rs.getLong("id"))
                .setTitle(rs.getString("title"))
                .setDestination(nullToEmpty(rs.getString("destination")))
                .setStartDate(start == null ? "" : start.toString())
                .setEndDate(end == null ? "" : end.toString())
                .setDescription(nullToEmpty(rs.getString("description")))
                .setCreatedBy(rs.getLong("created_by"))
                .build();
    }

    private Date toDate(String value)
    {
        if (value == null || value.isBlank())
        {
            return null;
        }
        return Date.valueOf(LocalDate.parse(value));
    }

    private String nullToEmpty(String value)
    {
        return value == null ? "" : value;
    }
}