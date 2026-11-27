package tagalong.server;

import io.grpc.Status;
import io.grpc.StatusRuntimeException;
import io.grpc.stub.StreamObserver;
import tagalong.dao.ActivityDAO;
import tagalong.dao.VoteDAO;
import tagalong.grpc.Activity;
import tagalong.grpc.ActivityVotes;
import tagalong.grpc.RemoveVoteRequest;
import tagalong.grpc.SetVoteRequest;
import tagalong.grpc.VoteServiceGrpc;
import tagalong.grpc.VoteValue;

import java.sql.SQLException;

public class VoteServiceImpl extends VoteServiceGrpc.VoteServiceImplBase
{
    private final ActivityDAO activityDAO;
    private final VoteDAO voteDAO;
    private final TripAccess tripAccess;

    public VoteServiceImpl(ActivityDAO activityDAO, VoteDAO voteDAO, TripAccess tripAccess)
    {
        this.activityDAO = activityDAO;
        this.voteDAO = voteDAO;
        this.tripAccess = tripAccess;
    }

    @Override
    public void setVote(SetVoteRequest request, StreamObserver<ActivityVotes> responseObserver)
    {
        if (request.getValue() != VoteValue.VOTE_VALUE_UP
                && request.getValue() != VoteValue.VOTE_VALUE_DOWN)
        {
            responseObserver.onError(Status.INVALID_ARGUMENT
                    .withDescription("value skal være UP eller DOWN")
                    .asRuntimeException());
            return;
        }

        try
        {
            checkAccess(request.getActivityId(), request.getUserId());

            voteDAO.setVote(request.getActivityId(), request.getUserId(), request.getValue());

            responseObserver.onNext(voteDAO.getVotes(request.getActivityId(), request.getUserId()));
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
    public void removeVote(RemoveVoteRequest request,
                           StreamObserver<ActivityVotes> responseObserver)
    {
        try
        {
            checkAccess(request.getActivityId(), request.getUserId());

            voteDAO.removeVote(request.getActivityId(), request.getUserId());

            responseObserver.onNext(voteDAO.getVotes(request.getActivityId(), request.getUserId()));
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

    private void checkAccess(long activityId, long userId) throws SQLException
    {
        Activity activity = activityDAO.findById(activityId).orElse(null);
        if (activity == null)
        {
            throw Status.NOT_FOUND.withDescription("Aktiviteten findes ikke").asRuntimeException();
        }
        tripAccess.checkMember(activity.getTripId(), userId);
    }

    private StatusRuntimeException internalError(SQLException e)
    {
        e.printStackTrace();
        return Status.INTERNAL.withDescription("Databasefejl").asRuntimeException();
    }
}