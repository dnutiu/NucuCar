package dev.nuculabs.nucuhub.domain;

import org.junit.Test;

import java.util.Locale;
import java.util.Random;

import static org.junit.Assert.*;


public class DeviceTest {
    @Test
    public void test_constructionValidTargets() {
        String[] testCases = {"http://localhost:8900/cool", "http://nuculabs.dev", "http://www.nuculabs.dev/", "http://user:pass@nuculabs.dev"};
        for (String target : testCases) {
            Device device = new Device(target);
            assertEquals(device.getTarget(), target);
        }
    }

    @Test
    public void test_constructionValidHostsAndPorts() {
        Random random = new Random();
        String[] testCases = {"http://localhost", "http://nuculabs.dev", "http://www.nuculabs.dev", "http://user:pass@nuculabs.dev"};
        for (String target : testCases) {
            int port = random.nextInt(65535);
            Device device = new Device(target, port);
            assertEquals(device.getTarget(), String.format(Locale.ENGLISH, "%s:%d", target, port));
        }
    }
}