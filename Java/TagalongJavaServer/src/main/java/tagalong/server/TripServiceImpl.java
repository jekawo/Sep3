package tagalong.server;

import tagalong.grpc.CreateTripRequest;
import tagalong.grpc.GetTripRequest;
import tagalong.grpc.ListMembersRequest;
import tagalong.grpc.ListMembersResponse;
import tagalong.grpc.ListTripsForUserRequest;
import tagalong.grpc.ListTripsResponse;
import tagalong.grpc.Trip;
import tagalong.grpc.TripServiceGrpc;
import io.grpc.Status;
import io.grpc.StatusRuntimeException;
import io.grpc.stub.StreamObserver;
import tagalong.dao.TripDAO;

import java.sql.SQLException;
import java.time.LocalDate;
import java.time.format.DateTimeParseException;

public class TripServiceImpl extends TripServiceGrpc.TripServiceImplBase
{
    private static final String FOREIGN_KEY_VIOLATION = "23503";

    private final TripDAO tripDAO;

    public TripServiceImpl(TripDAO tripDAO)
    {
        this.tripDAO = tripDAO;
    }

    @Override
    public void createTrip(CreateTripRequest request, StreamObserver<Trip> responseObserver)
    {
        if (request.getTitle().isBlank())
        {
            responseObserver.onError(invalid("Titel skal udfyldes"));
            return;
        }

        LocalDate start;
        LocalDate end;
        try
        {
            start = parseDate(request.getStartDate());
            end = parseDate(request.getEndDate());
        }
        catch (DateTimeParseException e)
        {
            responseObserver.onError(invalid("Datoer skal have formatet yyyy-MM-dd"));
            return;
        }

        if (start != null && end != null && end.isBefore(start))
        {
            responseObserver.onError(invalid("Slutdato kan ikke ligge før startdato!!!!"));
            return;
        }

        try
        {
            Trip trip = tripDAO.create(request.getRequestingUserId(),
                    request.getTitle().trim(), request.getDestination().trim(),
                    request.getStartDate(), request.getEndDate(), request.getDescription());
            responseObserver.onNext(trip);
            responseObserver.onCompleted();
        }
        catch (SQLException e)
        {
            if (FOREIGN_KEY_VIOLATION.equals(e.getSQLState()))
            {
                responseObserver.onError(Status.NOT_FOUND
                        .withDescription("Bruger findes ikke")
                        .asRuntimeException());
            }
            else
            {
                responseObserver.onError(internalError(e));
            }
        }
    }

    @Override
    public void getTrip(GetTripRequest request, StreamObserver<Trip> responseObserver)
    {
        try
        {
            checkMember(request.getTripId(), request.getRequestingUserId());
            Trip trip = tripDAO.findById(request.getTripId()).orElseThrow();
            responseObserver.onNext(trip);
            responseObserver.onCompleted();
        }
        catch (StatusRuntimeException e)
        {
            responseObserver.onError(e);
        }
        catch (SQLException e)
        {
            responseObserver.onError(internalError(e));
        }
    }

    @Override
    public void listTripsForUser(ListTripsForUserRequest request,
                                 StreamObserver<ListTripsResponse> responseObserver)
    {
        try
        {
            responseObserver.onNext(ListTripsResponse.newBuilder()
                    .addAllTrips(tripDAO.findByUser(request.getRequestingUserId()))
                    .build());
            responseObserver.onCompleted();
        }
        catch (SQLException e)
        {
            responseObserver.onError(internalError(e));
        }
    }

    @Override
    public void listMembers(ListMembersRequest request,
                            StreamObserver<ListMembersResponse> responseObserver)
    {
        try
        {
            checkMember(request.getTripId(), request.getRequestingUserId());
            responseObserver.onNext(ListMembersResponse.newBuilder()
                    .addAllMembers(tripDAO.findMembers(request.getTripId()))
                    .build());
            responseObserver.onCompleted();
        }
        catch (StatusRuntimeException e)
        {
            responseObserver.onError(e);
        }
        catch (SQLException e)
        {
            responseObserver.onError(internalError(e));
        }
    }

    private void checkMember(long tripId, long userId) throws SQLException
    {
        if (tripDAO.findById(tripId).isEmpty())
        {
            throw Status.NOT_FOUND.withDescription("Rejsen findes ikke").asRuntimeException();
        }
        if (!tripDAO.isMember(tripId, userId))
        {
            throw Status.PERMISSION_DENIED
                    .withDescription("Du er ikke medlem af denne rejse")
                    .asRuntimeException();
        }
    }

    private LocalDate parseDate(String value)
    {
        return value.isBlank() ? null : LocalDate.parse(value);
    }

    private StatusRuntimeException invalid(String message)
    {
        return Status.INVALID_ARGUMENT.withDescription(message).asRuntimeException();
    }

    private StatusRuntimeException internalError(SQLException e)
    {
        e.printStackTrace();
        return Status.INTERNAL.withDescription("Databasefejl").asRuntimeException();
    }
}