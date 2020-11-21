package dev.nuculabs.grpc;

import NucuCarSensorsProto.EnvironmentSensorGrpcServiceGrpc;
import NucuCarSensorsProto.NucuCarSensors;
import android.util.Log;
import io.grpc.ManagedChannel;
import io.grpc.ManagedChannelBuilder;
import io.grpc.StatusRuntimeException;

import java.util.Locale;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.ScheduledFuture;

import static java.util.concurrent.TimeUnit.SECONDS;


public class EnvironmentSensorService {
    private final String TAG = EnvironmentSensorService.class.getName();
    private final ManagedChannel channel;
    private final EnvironmentSensorGrpcServiceGrpc.EnvironmentSensorGrpcServiceBlockingStub blockingStub;
    private final ScheduledExecutorService scheduler = Executors.newScheduledThreadPool(1);
    private ScheduledFuture<?> periodicGrpcRequestHandle = null;
    private EnvironmentSensorData lastMeasurementData = new EnvironmentSensorData("{}", NucuCarSensors.SensorStateEnum.Uninitialized);

    public EnvironmentSensorService(String host, int port) {
        this(ManagedChannelBuilder.forAddress(host, port).usePlaintext());
        Log.i(TAG, String.format(Locale.ENGLISH, "Initializing channel host=%s port=%d", host, port));
    }

    public EnvironmentSensorService(ManagedChannelBuilder<?> channelBuilder) {
        channel = channelBuilder.build();
        blockingStub = EnvironmentSensorGrpcServiceGrpc.newBlockingStub(channel);
    }

    public void start() {
        final Runnable grpcPoolingTask = new Runnable() {
            public void run() {
                lastMeasurementData = getMeasurement();
            }
        };
        periodicGrpcRequestHandle = scheduler.scheduleAtFixedRate(grpcPoolingTask, 5, 10, SECONDS);
    }

    public void stop() {
        periodicGrpcRequestHandle.cancel(true);
    }

    public EnvironmentSensorData getLastMeasurementData() {
        return lastMeasurementData;
    }

    private EnvironmentSensorData getMeasurement() {
        try {
            NucuCarSensors.NucuCarSensorResponse response = null;
            response = blockingStub.getMeasurement(null);
            Log.d(TAG, "getMeasurement " + response.getJsonData());
            return new EnvironmentSensorData(response.getJsonData(), response.getState());
        } catch (StatusRuntimeException e) {
            Log.e(TAG, e.getLocalizedMessage());
            return new EnvironmentSensorData("{}", NucuCarSensors.SensorStateEnum.Error);
        }
    }
}
