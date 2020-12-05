package dev.nuculabs.nucuhub.ui.settings;

import android.os.Bundle;
import androidx.preference.ListPreference;
import androidx.preference.PreferenceFragmentCompat;
import dev.nuculabs.nucuhub.R;

public class SettingsFragment extends PreferenceFragmentCompat {
    @Override
    public void onCreatePreferences(Bundle savedInstanceState, String rootKey) {
        // To get preferences use this in main activity:
        // SharedPreferences sp = PreferenceManager.getDefaultSharedPreferences(this);

        setPreferencesFromResource(R.xml.root_preferences, rootKey);
        ListPreference devicesList = findPreference("device_list");

        // TODO: Override dialog in order to add a search devices button.
        //
        // set default value
    }
}
