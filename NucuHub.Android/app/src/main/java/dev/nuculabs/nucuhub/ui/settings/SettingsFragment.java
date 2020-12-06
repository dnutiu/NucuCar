package dev.nuculabs.nucuhub.ui.settings;

import android.app.Activity;
import android.content.SharedPreferences;
import android.os.Bundle;
import androidx.preference.PreferenceFragmentCompat;
import dev.nuculabs.nucuhub.R;

import java.util.Set;

public class SettingsFragment extends PreferenceFragmentCompat {
    private SharedPreferences sharedPreferences;


    @Override
    public void onCreatePreferences(Bundle savedInstanceState, String rootKey) {
        // To get preferences use this in main activity:
        // SharedPreferences sp = PreferenceManager.getDefaultSharedPreferences(this);

        setPreferencesFromResource(R.xml.root_preferences, rootKey);
        updateDeviceListItems();
    }

    private void updateDeviceListItems() {
        // dummy code to update device list entries
        SharedPreferences sp = getContext().getSharedPreferences("settings", Activity.MODE_PRIVATE);
        SharedPreferences.Editor spe = sp.edit();
        DeviceListPreference devicesList = findPreference("device_list");
        Set<String> savedEntries = sp.getStringSet("current_device_list", null);
        if (savedEntries != null) {
            CharSequence[] items = new CharSequence[savedEntries.size()];
            int index = 0;
            for (String item : savedEntries) {
                items[index] = item;
                index += 1;
            }
            devicesList.setEntries(items);
        }

    }
}
