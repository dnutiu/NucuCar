package dev.nuculabs.nucuhub.ui.settings;

import android.content.Context;
import android.util.AttributeSet;
import androidx.preference.ListPreference;

public class DeviceListPreference extends ListPreference {
    public DeviceListPreference(Context context, AttributeSet attrs, int defStyleAttr, int defStyleRes) {
        super(context, attrs, defStyleAttr, defStyleRes);
    }

    public DeviceListPreference(Context context, AttributeSet attrs, int defStyleAttr) {
        super(context, attrs, defStyleAttr);
    }

    public DeviceListPreference(Context context, AttributeSet attrs) {
        super(context, attrs);
    }

    public DeviceListPreference(Context context) {
        super(context);
    }

    @Override
    protected void onClick() {
        new DeviceManagementDialog(getContext()).show(this);
    }
}
