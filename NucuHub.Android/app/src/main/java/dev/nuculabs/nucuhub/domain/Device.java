package dev.nuculabs.nucuhub.domain;

import androidx.annotation.NonNull;

import java.util.Locale;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

/**
 * Device represents a NucuHub device.
 */
public class Device {
    private String target;

    public Device() {
    }

    /**
     * Constructs a new device instance.
     *
     * @param url Url of the form http://localhost:port
     */
    public Device(String url) {
        validateUrlAgainstRegex(url);
        target = url;
    }

    public Device(String host, int port) {
        String temp = String.format(Locale.ENGLISH, "%s:%d", host, port);
        validateUrlAgainstRegex(temp);
        target = temp;
    }

    public String getTarget() {
        return target;
    }

    public void setTarget(String target) {
        validateUrlAgainstRegex(target);
        this.target = target;
    }

    public boolean testConnection() {
        return true;
    }

    private void validateUrlAgainstRegex(String url) {
        String urlValidationRegex = "^(([^:/?#]+):)?(//([^/?#]*))?([^?#]*)(\\?([^#]*))?(#(.*))?";
        Pattern p = Pattern.compile(urlValidationRegex);
        Matcher m = p.matcher(url);
        if (m.matches()) {
            if (url.contains("http://") || url.contains("https://")) {
                throw new IllegalArgumentException("Don't include schema with URL");
            }
        } else {
            throw new IllegalArgumentException(String.format(Locale.ENGLISH,
                    "Malformed URL provided for device: %s", url));
        }
    }

    @NonNull
    @Override
    public String toString() {
        return this.target;
    }
}
