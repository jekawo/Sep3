package tagalong.server;

import io.grpc.Status;
import io.grpc.StatusRuntimeException;
import io.grpc.stub.StreamObserver;
import tagalong.dao.ActivityDAO;
import tagalong.dao.VoteDAO;
import tagalong.grpc.Activity;
import tagalong.grpc.ActivityServiceGrpc;
import tagalong.grpc.ActivityWithVotes;
import tagalong.grpc.CreateActivityRequest;
import tagalong.grpc.GetActivityRequest;
import tagalong.grpc.ListActivitiesRequest;
import tagalong.grpc.ListActivitiesResponse;

import java.sql.SQLException;

public class ActivityServiceImpl extends ActivityServiceGrpc.ActivityServiceImplBase
{
    private final ActivityDAO activityDAO;
    private final VoteDAO voteDAO;
    private final TripAccess tripAccess;

    public ActivityServiceImpl(ActivityDAO activityDAO, VoteDAO voteDAO, TripAccess tripAccess)
    {
        this.activityDAO = activityDAO;
        this.voteDAO = voteDAO;
        this.tripAccess = tripAccess;
    }

    @Override
    public void createActivity(CreateActivityRequest request,
                               StreamObserver<Activity> responseObserver)
    {
        if (request.getTitle().isBlank())
        {
            responseObserver.onError(invalid("Titel skal udfyldes"));
            return;
        }
        if (request.hasPrice() && request.getPrice() < 0)
        {
            responseObserver.onError(invalid("Prisen kan ikke være negativ"));
            return;
        }

        try
        {
            tripAccess.checkMember(request.getTripId(), request.getRequestingUserId());

            Activity activity = activityDAO.create(
                    request.getRequestingUserId(),
                    request.getTripId(),
                    request.getTitle().trim(),
                    request.getDescription(),
                    request.hasPrice() ? request.getPrice() : null,
                    request.getLocation(),
                    request.getLink());

            responseObserver.onNext(activity);
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
    public void getActivity(GetActivityRequest request,
                            StreamObserver<ActivityWithVotes> responseObserver)
    {
        try
        {
            Activity activity = activityDAO.findById(request.getActivityId()).orElse(null);
            if (activity == null)
            {
                responseObserver.onError(Status.NOT_FOUND
                        .withDescription("Aktiviteten findes ikke")
                        .asRuntimeException());
                return;
            }

            tripAccess.checkMember(activity.getTripId(), request.getRequestingUserId());

            responseObserver.onNext(withVotes(activity, request.getRequestingUserId()));
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
    public void listActivities(ListActivitiesRequest request,
                               StreamObserver<ListActivitiesResponse> responseObserver)
    {
        try
        {
            tripAccess.checkMember(request.getTripId(), request.getRequestingUserId());

            ListActivitiesResponse.Builder response = ListActivitiesResponse.newBuilder();
            for (Activity activity : activityDAO.findByTrip(request.getTripId()))
            {
                response.addActivities(withVotes(activity, request.getRequestingUserId()));
            }

            responseObserver.onNext(response.build());
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

    private ActivityWithVotes withVotes(Activity activity, long currentUserId)
            throws SQLException
    {
        return ActivityWithVotes.newBuilder()
                .setActivity(activity)
                .setVotes(voteDAO.getVotes(activity.getId(), currentUserId))
                .build();
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