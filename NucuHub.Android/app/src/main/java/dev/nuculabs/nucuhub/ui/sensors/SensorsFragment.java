package dev.nuculabs.nucuhub.ui.sensors;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;
import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.Observer;
import androidx.lifecycle.ViewModelProviders;
import com.google.android.material.chip.Chip;
import dev.nuculabs.nucuhub.domain.EnvironmentSensorData;
import dev.nuculabs.nucuhub.grpc.EnvironmentSensorService;
import dev.nuculabs.nucuhub.R;

public class SensorsFragment extends Fragment {
    private EnvironmentSensorViewModel environmentSensorViewModel;
    private EnvironmentSensorService environmentSensorService;
    // ui elements
    private ImageView sensorStatusImageView;
    private Chip sensorStatusText;
    private TextView temperatureText;
    private TextView humidityText;
    private TextView pressureText;
    private TextView vocText;

    public View onCreateView(@NonNull LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        environmentSensorViewModel = ViewModelProviders.of(this).get(EnvironmentSensorViewModel.class);
        View root = inflater.inflate(R.layout.fragment_environment_sensor, container, false);

        temperatureText = root.findViewById(R.id.envSensorTempText);
        humidityText = root.findViewById(R.id.envSensorHumidityText);
        pressureText = root.findViewById(R.id.envSensorPressureText);
        vocText = root.findViewById(R.id.envSensorVocText);
        sensorStatusImageView = root.findViewById(R.id.sensorStatusImageView);
        sensorStatusText = root.findViewById(R.id.sensorStatusChip);

        environmentSensorService = new EnvironmentSensorService("192.168.0.100", 8000);

        setupSensorStatusImage();
        setupSensorMeasurementsDisplays();
        setupEnvironmentSensorService();
        return root;
    }

    @Override
    public void onStart() {
        super.onStart();
        environmentSensorService.start();
    }

    @Override
    public void onStop() {
        super.onStop();
        environmentSensorService.stop();
    }

    private void setupSensorMeasurementsDisplays() {
        environmentSensorViewModel.getSensorMeasurementTemperature().observe(getViewLifecycleOwner(), new Observer<Double>() {
            @Override
            public void onChanged(Double value) {
                temperatureText.setText(value.toString());
            }
        });
        environmentSensorViewModel.getSensorMeasurementHumidity().observe(getViewLifecycleOwner(), new Observer<Double>() {
            @Override
            public void onChanged(Double value) {
                humidityText.setText(value.toString());
            }
        });
        environmentSensorViewModel.getSensorMeasurementPressure().observe(getViewLifecycleOwner(), new Observer<Double>() {
            @Override
            public void onChanged(Double value) {
                pressureText.setText(value.toString());
            }
        });
        environmentSensorViewModel.getSensorMeasurementVoc().observe(getViewLifecycleOwner(), new Observer<Double>() {
            @Override
            public void onChanged(Double value) {
                vocText.setText(value.toString());
            }
        });
    }

    private void setupEnvironmentSensorService() {
        environmentSensorService.getLastMeasurementData().observe(getViewLifecycleOwner(), new Observer<EnvironmentSensorData>() {
            @Override
            public void onChanged(EnvironmentSensorData environmentSensorData) {
                environmentSensorViewModel.notifyNewData(environmentSensorData);
            }
        });
    }

    private void setupSensorStatusImage() {
        // Dynamically observe the image of the sensor status image view.
        environmentSensorViewModel.getSensorStatusImage().observe(getViewLifecycleOwner(), new Observer<Integer>() {
            @Override
            public void onChanged(Integer resourceId) {
                sensorStatusImageView.setImageResource(resourceId);
            }
        });
        environmentSensorViewModel.getSensorStatusImageTooltip().observe(getViewLifecycleOwner(), new Observer<String>() {
            @Override
            public void onChanged(String tooltip) {
                sensorStatusImageView.setTooltipText(tooltip);
                sensorStatusText.setText(tooltip);
            }
        });
    }
}