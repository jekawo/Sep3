package tagalong.server;

import io.grpc.Status;
import io.grpc.StatusRuntimeException;
import io.grpc.stub.StreamObserver;
import tagalong.dao.InviteDAO;
import tagalong.dao.TripDAO;
import tagalong.grpc.AcceptInviteRequest;
import tagalong.grpc.CreateInviteRequest;
import tagalong.grpc.Invite;
import tagalong.grpc.InviteServiceGrpc;
import tagalong.grpc.Trip;

import java.security.SecureRandom;
import java.sql.SQLException;
import java.time.Instant;
import java.time.temporal.ChronoUnit;
import java.util.Base64;

public class InviteServiceImpl extends InviteServiceGrpc.InviteServiceImplBase
{
    private static final String FOREIGN_KEY_VIOLATION = "23503";
    private static final long DEFAULT_VALID_DAYS = 7;
    private static final long MAX_VALID_DAYS = 365;

    private final SecureRandom random = new SecureRandom();
    private final InviteDAO inviteDAO;
    private final TripDAO tripDAO;

    public InviteServiceImpl(InviteDAO inviteDAO, TripDAO tripDAO)
    {
        this.inviteDAO = inviteDAO;
        this.tripDAO = tripDAO;
    }

    @Override
    public void createInvite(CreateInviteRequest request, StreamObserver<Invite> responseObserver)
    {
        long validDays = request.getValidDays() == 0
                ? DEFAULT_VALID_DAYS
                : request.getValidDays();

        if (validDays < 0 || validDays > MAX_VALID_DAYS)
        {
            responseObserver.onError(Status.INVALID_ARGUMENT
                    .withDescription("valid_days skal være mellem 1 og " + MAX_VALID_DAYS)
                    .asRuntimeException());
            return;
        }

        try
        {
            checkMember(request.getTripId(), request.getRequestingUserId());

            Invite invite = inviteDAO.create(
                    request.getTripId(),
                    request.getRequestingUserId(),
                    generateToken(),
                    Instant.now().plus(validDays, ChronoUnit.DAYS));

            responseObserver.onNext(invite);
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
    public void acceptInvite(AcceptInviteRequest request, StreamObserver<Trip> responseObserver)
    {
        if (request.getToken().isBlank())
        {
            responseObserver.onError(Status.INVALID_ARGUMENT
                    .withDescription("token skal udfyldes")
                    .asRuntimeException());
            return;
        }

        try
        {
            Invite invite = inviteDAO.findByToken(request.getToken()).orElse(null);
            if (invite == null)
            {
                responseObserver.onError(Status.NOT_FOUND
                        .withDescription("Invitationen findes ikke")
                        .asRuntimeException());
                return;
            }

            if (invite.getExpiresAt().getSeconds() < Instant.now().getEpochSecond())
            {
                responseObserver.onError(Status.FAILED_PRECONDITION
                        .withDescription("Invitationen er udløbet")
                        .asRuntimeException());
                return;
            }

            tripDAO.addMember(invite.getTripId(), request.getUserId());

            Trip trip = tripDAO.findById(invite.getTripId()).orElseThrow();
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

    private String generateToken()
    {
        byte[] bytes = new byte[32];
        random.nextBytes(bytes);
        return Base64.getUrlEncoder().withoutPadding().encodeToString(bytes);
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

    private StatusRuntimeException internalError(SQLException e)
    {
        e.printStackTrace();
        return Status.INTERNAL.withDescription("Databasefejl").asRuntimeException();
    }
}