package dev.nuculabs.nucuhub.ui.settings.device;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.TextView;
import dev.nuculabs.nucuhub.domain.Device;

import java.util.ArrayList;

public class DeviceListAdapter extends BaseAdapter  {
    private final ArrayList<Device> items = new ArrayList<>();
    private final Context context;
    private final LayoutInflater inflater;


    public DeviceListAdapter(Context context) {
        this.context = context;
        this.inflater = LayoutInflater.from(context);
    }

    public DeviceListAdapter(ArrayList<Device> items, Context context) {
        this(context);
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
        final TextView text;
        if (view == null) {
            view = inflater.inflate(android.R.layout.simple_list_item_1, null);
        }

        text = (TextView) view;
        text.setText(items.get(position).toString());

        return view;
    }

    public boolean add(Device device) {
        return items.add(device);
    }
}
