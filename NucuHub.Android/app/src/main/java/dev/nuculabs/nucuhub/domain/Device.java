package dev.nuculabs.nucuhub.domain;

import androidx.annotation.NonNull;

import java.net.MalformedURLException;
import java.net.URL;
import java.util.Locale;

/**
 * Device represents a NucuHub device.
 */
public class Device {
    private URL target;

    public Device() {
    }

    /**
     * Constructs a new device instance.
     *
     * @param url Url of the form http://localhost:port
     */
    public Device(String url) {
        stringToURL(url);
    }

    public Device(String host, int port) {
        String temp = String.format(Locale.ENGLISH, "%s:%d", host, port);
        stringToURL(temp);
    }

    public String getTarget() {
        return target.toString();
    }

    public void setTarget(String target) {
        stringToURL(target);
    }

    public boolean testConnection() {
        return true;
    }

    private void stringToURL(String url) {
        try {
            target = new URL(url);
        } catch (MalformedURLException e) {
            throw new IllegalArgumentException(e.getMessage());
        }
    }

    @NonNull
    @Override
    public String toString() {
        return this.target.toString();
    }
}
