package dev.nuculabs.nucuhub.ui.sensors;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageView;
import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.Observer;
import androidx.lifecycle.ViewModelProviders;
import dev.nuculabs.grpc.EnvironmentSensorService;
import dev.nuculabs.nucuhub.R;

import java.util.logging.Logger;

public class SensorsFragment extends Fragment {
    private static final Logger logger = Logger.getLogger(SensorsFragment.class.getName());
    private View root;
    private EnvironmentSensorViewModel environmentSensorViewModel;

    public View onCreateView(@NonNull LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        environmentSensorViewModel = ViewModelProviders.of(this).get(EnvironmentSensorViewModel.class);
        root = inflater.inflate(R.layout.fragment_environment_sensor, container, false);

        // test button TODO: Investigate lifecycle methods and stop/start the scheduler accordingly.
        Button testButton = root.findViewById(R.id.grpc_test_button);
        final EnvironmentSensorService client = new EnvironmentSensorService("192.168.0.101", 8000);
        client.start();
        testButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                logger.info("test button clicked!");
                client.stop();
            }
        });

        setupSensorStatusImage();
        return root;
    }


    private void setupSensorStatusImage() {
        // Dynamically observe the image of the sensor status image view.
        final ImageView sensorStatusImageView = root.findViewById(R.id.sensorStatusImageView);
        environmentSensorViewModel.getSensorStatusImageResourceId().observe(getViewLifecycleOwner(), new Observer<Integer>() {
            @Override
            public void onChanged(Integer resourceId) {
                sensorStatusImageView.setImageResource(resourceId);
            }
        });
        environmentSensorViewModel.getSensorStatusImageTooltip().observe(getViewLifecycleOwner(), new Observer<String>() {
            @Override
            public void onChanged(String tooltip) {
                sensorStatusImageView.setTooltipText(tooltip);
            }
        });
    }
}