package dev.nuculabs.nucuhub.ui.sensors;

import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;
import androidx.lifecycle.ViewModel;
import dev.nuculabs.grpc.EnvironmentSensorData;
import dev.nuculabs.nucuhub.R;

public class EnvironmentSensorViewModel extends ViewModel {

    private final MutableLiveData<String> sensorStatusImageTooltip;
    private final MutableLiveData<Integer> sensorStatusImage;
    private final MutableLiveData<Double> sensorMeasurementTemperature;
    private final MutableLiveData<Double> sensorMeasurementHumidity;
    private final MutableLiveData<Double> sensorMeasurementPressure;
    private final MutableLiveData<Double> sensorMeasurementVoc;

    public LiveData<Double> getSensorMeasurementTemperature() {
        return sensorMeasurementTemperature;
    }

    public LiveData<Double> getSensorMeasurementHumidity() {
        return sensorMeasurementHumidity;
    }

    public LiveData<Double> getSensorMeasurementPressure() {
        return sensorMeasurementPressure;
    }

    public LiveData<Double> getSensorMeasurementVoc() {
        return sensorMeasurementVoc;
    }

    public EnvironmentSensorViewModel() {
        // image.
        sensorStatusImage = new MutableLiveData<>();
        sensorStatusImage.setValue(R.drawable.no_signal);
        sensorStatusImageTooltip = new MutableLiveData<>();
        // measurements
        sensorMeasurementTemperature = new MutableLiveData<>();
        sensorMeasurementHumidity = new MutableLiveData<>();
        sensorMeasurementPressure = new MutableLiveData<>();
        sensorMeasurementVoc = new MutableLiveData<>();
    }

    public void notifyNewData(EnvironmentSensorData data) {
        updateStatusImage(data);
        sensorMeasurementTemperature.setValue(data.getTemperature());
        sensorMeasurementHumidity.setValue(data.getHumidity());
        sensorMeasurementPressure.setValue(data.getPressure());
        sensorMeasurementVoc.setValue(data.getVolatileOrganicCompounds());
        updateStatusImageTooltipValue();
    }

    public LiveData<Integer> getSensorStatusImage() {
        return sensorStatusImage;
    }

    public LiveData<String> getSensorStatusImageTooltip() {
        return sensorStatusImageTooltip;
    }

    private void updateStatusImage(EnvironmentSensorData data) {
        int airQualityScore = data.computeAirQualityScore();
        if (airQualityScore >= 0 && airQualityScore <= 1) {
            sensorStatusImage.setValue(R.drawable.status_good);
        } else if (airQualityScore == 2) {
            sensorStatusImage.setValue(R.drawable.status_neutral);
        } else if (airQualityScore >= 3) {
            sensorStatusImage.setValue(R.drawable.status_bad);
        } else {
            sensorStatusImage.setValue(R.drawable.no_signal);
        }
    }

    private void updateStatusImageTooltipValue() {
        Integer sensorStatusImageResourceId = this.sensorStatusImage.getValue();
        if (sensorStatusImageResourceId == null) {
            sensorStatusImageResourceId = -1;
        }

        switch (sensorStatusImageResourceId) {
            case R.drawable.status_good:
                sensorStatusImageTooltip.setValue("The air quality is good.");
                break;
            case R.drawable.status_neutral:
                sensorStatusImageTooltip.setValue("The air quality is not great, not terrible.");
                break;
            case R.drawable.status_bad:
                sensorStatusImageTooltip.setValue("The air quality is bad.");
                break;
            case R.drawable.no_signal:
            default:
                sensorStatusImageTooltip.setValue("No signal. Please ensure device is connected!");
                break;
        }
    }
}