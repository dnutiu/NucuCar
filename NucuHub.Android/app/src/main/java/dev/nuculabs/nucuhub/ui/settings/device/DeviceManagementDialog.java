package dev.nuculabs.nucuhub.ui.settings.device;

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
import android.widget.EditText;
import android.widget.ListView;
import androidx.annotation.NonNull;
import androidx.appcompat.widget.Toolbar;
import androidx.preference.ListPreference;
import com.google.android.material.textfield.TextInputLayout;
import dev.nuculabs.nucuhub.R;
import dev.nuculabs.nucuhub.domain.SettingValues;

import java.util.HashSet;
import java.util.Objects;

public class DeviceManagementDialog extends Dialog {
    private SharedPreferences sharedPreferences;
    private ListPreference preference = null;
    private ArrayAdapter<String> adapter;
    private TextInputLayout deviceTextInputLayout;

    public DeviceManagementDialog(@NonNull Context context) {
        super(context, R.style.ThemeOverlay_AppCompat_ActionBar);
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.settings_device_management_dialog);
        getWindow().setLayout(WindowManager.LayoutParams.MATCH_PARENT, WindowManager.LayoutParams.MATCH_PARENT);
        setupToolbar();

        sharedPreferences = getContext().getSharedPreferences(SettingValues.NAME, Activity.MODE_PRIVATE);
        final ListView deviceListView = requireViewById(R.id.settings_device_dialog_list);
        final Button addDeviceButton = requireViewById(R.id.settings_device_add_button);
        deviceTextInputLayout = requireViewById(R.id.settings_device_input_device);
        adapter = new ArrayAdapter<>(getContext(), android.R.layout.simple_list_item_1);

        deviceListView.setAdapter(adapter);

        // Add the existing items in the list adapter so they will be displayed.
        final CharSequence [] preferenceCharSeq = preference.getEntries();
        for (CharSequence item : preferenceCharSeq) {
            adapter.add(item.toString());
        }

        // on-click handlers.
        addDeviceButton.setOnClickListener(new AddNewDeviceAction());
        // TODO add support for selecting device and deleting.
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
        int itemsLength = adapter.getCount();
        CharSequence[] entries = new CharSequence[itemsLength];
        HashSet<String> entriesSet = new HashSet<>();
        for (int i = 0; i < itemsLength; i++) {
            entries[i] = adapter.getItem(i);
            entriesSet.add(entries[i].toString());
        }
        preference.setEntries(entries);

        SharedPreferences.Editor spe = sharedPreferences.edit();
        spe.putStringSet(SettingValues.CURRENT_DEVICE_LIST, entriesSet);
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

    private class AddNewDeviceAction implements View.OnClickListener {

        @Override
        public void onClick(View v) {
            // TODO: Test connection before adding. loading -> testing connection -> show dialog.
            EditText editText = Objects.requireNonNull(deviceTextInputLayout.getEditText());
            String target = editText.getText().toString();
            adapter.add(target);
            editText.setText(null);
        }
    }
}
