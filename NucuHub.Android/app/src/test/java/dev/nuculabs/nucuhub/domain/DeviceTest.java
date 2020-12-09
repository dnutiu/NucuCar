package dev.nuculabs.nucuhub.domain;

import org.junit.Test;

import java.util.Locale;
import java.util.Random;

import static org.junit.Assert.*;


public class DeviceTest {
    @Test
    public void test_constructionValidTargets() {
        String[] testCases = {"localhost:8900/cool", "nuculabs.dev", "www.nuculabs.dev/", "user:pass@nuculabs.dev"};
        for (String target : testCases) {
            Device device = new Device(target);
            assertEquals(device.getTarget(), target);
        }
    }

    @Test
    public void test_constructionValidHostsAndPorts() {
        Random random = new Random();
        String[] testCases = {"localhost", "nuculabs.dev", "www.nuculabs.dev", "user:pass@nuculabs.dev"};
        for (String target : testCases) {
            int port = random.nextInt(65535);
            Device device = new Device(target, port);
            assertEquals(device.getTarget(), String.format(Locale.ENGLISH,"%s:%d", target, port));
        }
    }

    @Test(expected = IllegalArgumentException.class)
    public void test_constructionInvalidHostAndPortHttps() {
        new Device("https://google.com", 443);
    }

    @Test(expected = IllegalArgumentException.class)
    public void test_constructionInvalidHostAndPortHttp() {
        new Device("http://google.com", 443);
    }

    @Test(expected = IllegalArgumentException.class)
    public void test_constructionInvalidTargetHttp() {
        new Device("http://localhost:8900/cool");
    }

    @Test(expected = IllegalArgumentException.class)
    public void test_constructionInvalidTargetHttps() {
        new Device("https://nuculabs.dev");
    }
}