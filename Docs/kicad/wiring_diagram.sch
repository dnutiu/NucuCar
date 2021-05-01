EESchema Schematic File Version 4
EELAYER 30 0
EELAYER END
$Descr A4 11693 8268
encoding utf-8
Sheet 1 1
Title ""
Date ""
Rev ""
Comp ""
Comment1 ""
Comment2 ""
Comment3 ""
Comment4 ""
$EndDescr
$Comp
L MCU_Module:RaspberryPi-CM3 U?
U 1 1 608D7428
P 2850 4150
F 0 "U?" H 2850 669 50  0000 C CNN
F 1 "RaspberryPi-CM3" H 2850 760 50  0000 C CNN
F 2 "" H 2250 7550 50  0001 C CNN
F 3 "https://www.raspberrypi.org/documentation/hardware/computemodule/datasheets/rpi_DATA_CM_1p0.pdf" H 2250 7550 50  0001 C CNN
	1    2850 4150
	-1   0    0    1   
$EndComp
$Comp
L Sensor:BME680 U?
U 1 1 608E21CB
P 6000 1500
F 0 "U?" H 5571 1546 50  0000 R CNN
F 1 "BME680" H 5571 1455 50  0000 R CNN
F 2 "Package_LGA:Bosch_LGA-8_3x3mm_P0.8mm_ClockwisePinNumbering" H 7450 1050 50  0001 C CNN
F 3 "https://ae-bst.resource.bosch.com/media/_tech/media/datasheets/BST-BME680-DS001.pdf" H 6000 1300 50  0001 C CNN
	1    6000 1500
	1    0    0    -1  
$EndComp
Text Notes 7350 7500 0    50   ~ 0
NucuSensors Wiring Diagram
Text Notes 8300 7650 0    50   ~ 0
2021-05-01
$Comp
L pms5003-rescue:PMS5003-RESCUE-pms5003 J?
U 1 1 608E4563
P 4750 3150
F 0 "J?" H 4532 3211 50  0000 R CNN
F 1 "PMS5003-RESCUE-pms5003" H 4532 3120 50  0000 R CNN
F 2 "" H 4750 3300 50  0001 C CNN
F 3 "" H 4750 3300 50  0001 C CNN
	1    4750 3150
	-1   0    0    -1  
$EndComp
Wire Wire Line
	3850 3850 3950 3850
Wire Wire Line
	4000 3350 4000 3000
Wire Wire Line
	4000 3000 4600 3000
Wire Wire Line
	3850 4850 4250 4850
Wire Wire Line
	4250 3100 4600 3100
Wire Wire Line
	3850 4650 4400 4650
Wire Wire Line
	4400 3200 4600 3200
Wire Wire Line
	3850 5050 4100 5050
Wire Wire Line
	4100 5050 4100 2900
Wire Wire Line
	4100 2900 4600 2900
Wire Wire Line
	3850 5250 4350 5250
Wire Wire Line
	4350 5250 4350 2800
Wire Wire Line
	4350 2800 4600 2800
Text Label 4000 5250 0    50   ~ 0
5V
Text Label 3900 5050 0    50   ~ 0
GND
Wire Wire Line
	4250 3100 4250 4850
Wire Wire Line
	4400 3200 4400 4650
Text Label 3850 4850 0    50   ~ 0
BCM14
Text Label 3850 4750 0    50   ~ 0
BCM15
Wire Wire Line
	3950 4050 3950 3850
Wire Wire Line
	3950 3350 4000 3350
Wire Wire Line
	4450 3300 4450 3850
Wire Wire Line
	4450 3300 4600 3300
Wire Wire Line
	3950 4050 3850 4050
Text Label 3900 4050 0    50   ~ 0
BCM23
Text Label 3950 3850 0    50   ~ 0
BCM24
Connection ~ 3950 3850
Wire Wire Line
	3950 3850 4450 3850
Wire Wire Line
	3950 3850 3950 3350
Wire Wire Line
	3850 5550 5250 5550
Wire Wire Line
	5250 5550 5250 850 
Wire Wire Line
	5250 850  6100 850 
Wire Wire Line
	6100 850  6100 900 
Text Label 4300 5500 0    50   ~ 0
3.3V
Wire Wire Line
	4100 5050 5800 5050
Wire Wire Line
	5800 5050 5800 2100
Wire Wire Line
	5800 2100 5900 2100
Connection ~ 4100 5050
Wire Wire Line
	6600 1400 6750 1400
Wire Wire Line
	6750 1400 6750 5350
Wire Wire Line
	6750 5350 3850 5350
Text Label 4450 5350 0    50   ~ 0
SDA
Wire Wire Line
	3850 5150 6950 5150
Wire Wire Line
	6950 5150 6950 1600
Wire Wire Line
	6950 1600 6600 1600
Text Label 6200 5150 0    50   ~ 0
SCL
$EndSCHEMATC
