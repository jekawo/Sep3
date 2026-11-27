package tagalong.dao;

import tagalong.grpc.Activity;

import java.sql.SQLException;
import java.util.List;
import java.util.Optional;

public interface ActivityDAO
{
    Activity create(long userId, long tripId, String title, String description,
                    Double price, String location, String link) throws SQLException;

    Optional<Activity> findById(long activityId) throws SQLException;

    List<Activity> findByTrip(long tripId) throws SQLException;
}