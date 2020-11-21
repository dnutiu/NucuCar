package dev.nuculabs.nucuhub.ui.sensors;

import androidx.lifecycle.LiveData;
import androidx.lifecycle.MutableLiveData;
import androidx.lifecycle.ViewModel;
import dev.nuculabs.nucuhub.R;

public class EnvironmentSensorViewModel extends ViewModel {

    private MutableLiveData<String> sensorStatusImageTooltip;
    private MutableLiveData<Integer> sensorStatusImageResourceId;

    public EnvironmentSensorViewModel() {
        sensorStatusImageResourceId = new MutableLiveData<>();
        sensorStatusImageResourceId.setValue(R.drawable.no_signal);
        sensorStatusImageTooltip = new MutableLiveData<>();
    }

    public LiveData<Integer> getSensorStatusImageResourceId() {
        return sensorStatusImageResourceId;
    }

    public LiveData<String> getSensorStatusImageTooltip() {
        Integer sensorStatusImageResourceId = this.sensorStatusImageResourceId.getValue();
        if (sensorStatusImageResourceId == null) {
            sensorStatusImageResourceId = -1;
        }

        switch (sensorStatusImageResourceId) {
            case R.drawable.status_good:
                sensorStatusImageTooltip.setValue("The air quality is good.");
            case R.drawable.status_netural:
                sensorStatusImageTooltip.setValue("The air quality is not great, not terrible.");
            case R.drawable.status_bad:
                sensorStatusImageTooltip.setValue("The air quality is bad.");
            case R.drawable.no_signal:
            default:
                sensorStatusImageTooltip.setValue("No signal. Please ensure device is connected!");
                break;
        }
        return sensorStatusImageTooltip;
    }
}