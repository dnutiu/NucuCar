package dev.nuculabs.nucuhub.ui.settings;

import android.app.Activity;
import android.content.SharedPreferences;
import android.os.Bundle;
import androidx.preference.PreferenceFragmentCompat;
import dev.nuculabs.nucuhub.R;
import dev.nuculabs.nucuhub.domain.SettingValues;
import dev.nuculabs.nucuhub.ui.settings.device.DeviceListPreference;

import java.util.Objects;
import java.util.Set;

public class SettingsFragment extends PreferenceFragmentCompat {
    public static final String DEVICE_LIST_PREFERENCE = "device_list";

    @Override
    public void onCreatePreferences(Bundle savedInstanceState, String rootKey) {
        // To get preferences use this in main activity:
        // SharedPreferences sp = PreferenceManager.getDefaultSharedPreferences(this);

        setPreferencesFromResource(R.xml.root_preferences, rootKey);
        initializeDeviceListItems();
    }

    private void initializeDeviceListItems() {
        SharedPreferences sp = Objects.requireNonNull(getContext()).getSharedPreferences(SettingValues.NAME, Activity.MODE_PRIVATE);
        DeviceListPreference devicesList = findPreference(DEVICE_LIST_PREFERENCE);
        Set<String> savedEntries = sp.getStringSet(SettingValues.DEVICE_LIST, null);
        if (savedEntries != null) {
            CharSequence[] items = new CharSequence[savedEntries.size()];
            int index = 0;
            for (String item : savedEntries) {
                items[index] = item;
                index += 1;
            }
            devicesList.setEntries(items);
        }
        // TODO: Set default device.
    }
}
