package dev.nuculabs.nucuhub.ui.settings;

import android.app.Activity;
import android.app.Dialog;
import android.content.Context;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.view.MenuItem;
import android.view.View;
import android.view.WindowManager;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ListView;
import androidx.annotation.NonNull;
import androidx.appcompat.widget.Toolbar;
import androidx.preference.ListPreference;
import dev.nuculabs.nucuhub.R;

import java.util.HashSet;

public class DeviceManagementDialog extends Dialog {
    private SharedPreferences sharedPreferences;
    private ListPreference preference = null;
    private ArrayAdapter<String> adapter;

    public DeviceManagementDialog(@NonNull Context context) {
        super(context, R.style.ThemeOverlay_AppCompat_ActionBar);
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.settings_device_management_dialog);
        getWindow().setLayout(WindowManager.LayoutParams.MATCH_PARENT, WindowManager.LayoutParams.MATCH_PARENT);
        setupToolbar();

        sharedPreferences = getContext().getSharedPreferences("settings", Activity.MODE_PRIVATE);

        final ListView deviceListView = findViewById(R.id.settings_device_dialog_list);
        adapter = new ArrayAdapter<String>(getContext(), android.R.layout.simple_list_item_1);
        deviceListView.setAdapter(adapter);

        // dummy code to add simple items.
        final CharSequence [] preferenceCharSeq = preference.getEntries();
        for (CharSequence item : preferenceCharSeq) {
            adapter.add(item.toString());
        }

        Button testConnectionButton = findViewById(R.id.settings_device_test_connection_button);
        testConnectionButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                adapter.add("new object.");
            }
        });
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // back button click
        if (item.getItemId() == android.R.id.home) {
            saveChangesAndDismiss();
            return true;
        }

        return super.onOptionsItemSelected(item);
    }

    @Override
    public void onBackPressed() {
        saveChangesAndDismiss();
        super.onBackPressed();
    }

    public void show(@NonNull ListPreference preference) {
        this.preference = preference;
        super.show();
    }

    public void saveChangesAndDismiss() {
        updatePreferenceEntryValues();
        dismiss();
    }

    private void updatePreferenceEntryValues() {
        // dummy code to update the entries.
        int itemsLength = adapter.getCount();
        CharSequence[] entries = new CharSequence[itemsLength];
        HashSet<String> entriesSet = new HashSet<>();
        for (int i = 0; i < itemsLength; i++) {
            entries[i] = adapter.getItem(i);
            entriesSet.add(entries[i].toString());
        }
        preference.setEntries(entries);

        SharedPreferences.Editor spe = sharedPreferences.edit();
        spe.putStringSet("current_device_list", entriesSet);
    }

    private void setupToolbar() {
        Toolbar toolbar = findViewById(R.id.settings_device_dialog_toolbar);
        toolbar.setTitle(R.string.device_management_title);
        toolbar.setNavigationIcon(R.drawable.ic_baseline_arrow_gray_24);
        toolbar.setNavigationOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                saveChangesAndDismiss();
            }
        });
    }
}
