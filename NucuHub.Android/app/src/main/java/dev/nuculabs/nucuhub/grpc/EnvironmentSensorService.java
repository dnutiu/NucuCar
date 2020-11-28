package dev.nuculabs.nucuhub.grpc;

import NucuCarSensorsProto.EnvironmentSensorGrpcServiceGrpc;
import NucuCarSensorsProto.NucuCarSensors;
import android.util.Log;
import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;
import dev.nuculabs.nucuhub.domain.EnvironmentSensorData;
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
    private final EnvironmentSensorGrpcServiceGrpc.EnvironmentSensorGrpcServiceBlockingStub blockingStub;
    private final ScheduledExecutorService scheduler = Executors.newScheduledThreadPool(1);
    private ScheduledFuture<?> periodicGrpcRequestHandle = null;
    private final MutableLiveData<EnvironmentSensorData> lastMeasurementData;

    public EnvironmentSensorService(String host, int port) {
        this(ManagedChannelBuilder.forAddress(host, port).usePlaintext());
        Log.i(TAG, String.format(Locale.ENGLISH, "Initializing channel host=%s port=%d", host, port));
    }

    public EnvironmentSensorService(ManagedChannelBuilder<?> channelBuilder) {
        ManagedChannel channel = channelBuilder.build();
        blockingStub = EnvironmentSensorGrpcServiceGrpc.newBlockingStub(channel);
        lastMeasurementData = new MutableLiveData<>();
        lastMeasurementData.setValue(new EnvironmentSensorData("{}", NucuCarSensors.SensorStateEnum.Uninitialized));
    }

    public void start() {
        final Runnable grpcPoolingTask = new Runnable() {
            public void run() {
                try {
                    EnvironmentSensorData data = getMeasurement();
                    Log.i(TAG, "Got new data " + data.toString());
                    lastMeasurementData.postValue(data);
                } catch (Exception e) {
                    Log.e(TAG, e.getLocalizedMessage());
                }
            }
        };
        Log.i(TAG, "Scheduling automatic update.");
        periodicGrpcRequestHandle = scheduler.scheduleAtFixedRate(grpcPoolingTask, 0, 10, SECONDS);
    }

    public void stop() {
        Log.i(TAG, "Stopping automatic update.");
        periodicGrpcRequestHandle.cancel(true);
    }

    public LiveData<EnvironmentSensorData> getLastMeasurementData() {
        return lastMeasurementData;
    }

    private EnvironmentSensorData getMeasurement() {
        try {
            NucuCarSensors.NucuCarSensorResponse response = blockingStub.getMeasurement(null);
            Log.d(TAG, "getMeasurement " + response.getJsonData());
            return new EnvironmentSensorData(response.getJsonData(), response.getState());
        } catch (StatusRuntimeException e) {
            Log.e(TAG, e.getLocalizedMessage());
            return new EnvironmentSensorData("{}", NucuCarSensors.SensorStateEnum.Error);
        }
    }
}
