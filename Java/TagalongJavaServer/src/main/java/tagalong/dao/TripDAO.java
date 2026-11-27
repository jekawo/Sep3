package tagalong.dao;

import tagalong.grpc.Member;
import tagalong.grpc.Trip;

import java.sql.SQLException;
import java.util.List;
import java.util.Optional;

public interface TripDAO
{
    Trip create(long userId, String title, String destination,
                String startDate, String endDate, String description) throws SQLException;

    Optional<Trip> findById(long tripId) throws SQLException;

    List<Trip> findByUser(long userId) throws SQLException;

    List<Member> findMembers(long tripId) throws SQLException;

    boolean isMember(long tripId, long userId) throws SQLException;

    void addMember(long tripId, long userId) throws SQLException;
}