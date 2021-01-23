package dev.nuculabs.nucuhub.ui.settings.device;

import android.content.Context;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.ImageButton;
import android.widget.LinearLayout;
import android.widget.TextView;
import dev.nuculabs.nucuhub.R;
import dev.nuculabs.nucuhub.domain.Device;

import java.util.ArrayList;

public class DeviceListAdapter extends BaseAdapter  {
    private final String TAG = DeviceListAdapter.class.getName();
    private final ArrayList<Device> items = new ArrayList<>();
    private final LayoutInflater inflater;
    private final DeleteDeviceAction deleteDeviceAction;
    private final StarDeviceAction starDeviceAction;
    private final DeviceManagementDialog deviceManagementDialog;


    public DeviceListAdapter(Context context, DeviceManagementDialog dmd) {
        this.inflater = LayoutInflater.from(context);
        this.deleteDeviceAction = new DeleteDeviceAction();
        this.starDeviceAction = new StarDeviceAction();
        this.deviceManagementDialog = dmd;
    }

    public DeviceListAdapter(ArrayList<Device> items, Context context, DeviceManagementDialog dmd) {
        this(context, dmd);
        this.items.addAll(items);
    }

    @Override
    public int getCount() {
        return items.size();
    }

    @Override
    public Device getItem(int position) {
        return items.get(position);
    }

    @Override
    public long getItemId(int position) {
        return items.get(position).hashCode();
    }

    @Override
    public View getView(int position, View convertView, ViewGroup parent) {
        View view = convertView;
        if (view == null) {
            view = inflater.inflate(R.layout.settings_device_list_item, null);
        }
        final TextView text = view.findViewById(R.id.settings_device_list_item_text);
        final TextView devicePosition = view.findViewById(R.id.settings_device_list_item_position);
        final ImageButton deleteButton = view.findViewById(R.id.settings_device_list_item_delete_button);
        final ImageButton starButton = view.findViewById(R.id.settings_device_list_item_select_button);

        text.setText(items.get(position).toString());
        devicePosition.setText(Integer.toString(position));

        deleteButton.setOnClickListener(deleteDeviceAction);
        starButton.setOnClickListener(starDeviceAction);

        return view;
    }

    @Override
    public void notifyDataSetChanged() {
        super.notifyDataSetChanged();
        deviceManagementDialog.updatePreferenceEntryValues();
    }

    public boolean add(Device device) {
        Log.d(TAG, "Adding item " + device.toString());
        return items.add(device);
    }

    private class DeleteDeviceAction implements View.OnClickListener {
        @Override
        public void onClick(View v) {
            View parentContainer = (View) v.getParent().getParent();
            TextView positionText = parentContainer.findViewById(R.id.settings_device_list_item_position);
            items.remove(Integer.parseInt(positionText.getText().toString()));
            notifyDataSetChanged();
        }
    }

    private class StarDeviceAction implements View.OnClickListener {
        private final String TAG = StarDeviceAction.class.getName();

        @Override
        public void onClick(View v) {
            // TODO: Highlight selected item row?
            LinearLayout parent = (LinearLayout) v.getParent().getParent();
            CharSequence text = ((TextView) parent.getChildAt(0)).getText();
            Log.i(TAG, "StarDeviceAction " + text.toString());
        }
    }
}
