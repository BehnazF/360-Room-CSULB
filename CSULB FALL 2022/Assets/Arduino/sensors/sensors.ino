#include <Arduino.h>
#include <stdint.h>

/**
 * PIN Configuration
 */
#define LED_PIN 13
#define BUTTON_PIN 3
#define BUTTON_PIN2 4
// Pin labeled "trig" on the ultrasonic sensor
#define ULTRASONIC_TRIGGER_PIN 11
// Pin labeled "echo" on the ultrasonic sensor
#define ULTRASONIC_ECHO_PIN 12

/**
 * Data for messages
 */
#define MSG_HEADER 254
enum SensorKind : uint8_t
{
  SensorKind_NONE = 0,
  SensorKind_BUTTON = 1,
  SensorKind_ULTRASONIC = 2,
  SensorKind_BUTTON2 = 3,
};

// These functions are implemented at the bottom of the file
// They extract the Lo and Hi bytes of uint16_t integers, which have 2 bytes.
uint8_t getLoByte(uint16_t value);
uint8_t getHiByte(uint16_t value);

void setup()
{
  Serial.begin(9600);

  pinMode(LED_PIN, OUTPUT);

  pinMode(BUTTON_PIN, INPUT);
  pinMode(BUTTON_PIN2, INPUT);

  pinMode(ULTRASONIC_TRIGGER_PIN, OUTPUT);
  pinMode(ULTRASONIC_ECHO_PIN, INPUT);
}

uint32_t last_send_time_milliseconds = 0;

uint32_t last_ultrasonic_reading_milliseconds = 0;
uint32_t last_ultrasonic_distance_cm = 0;
void loop()
{
  uint32_t now_milliseconds = millis();

  if (now_milliseconds - last_ultrasonic_reading_milliseconds > 250)
  { // Update ultrasonic sensor every 250 ms
    last_ultrasonic_reading_milliseconds = now_milliseconds;
    // The sensor is triggered by a HIGH pulse of 10 or more microseconds.
    // Give a short LOW pulse beforehand to ensure a clean HIGH pulse:
    digitalWrite(ULTRASONIC_TRIGGER_PIN, LOW);
    delayMicroseconds(5);
    digitalWrite(ULTRASONIC_TRIGGER_PIN, HIGH);
    delayMicroseconds(10);
    digitalWrite(ULTRASONIC_TRIGGER_PIN, LOW);

    // Read the signal from the sensor: a HIGH pulse whose
    // duration is the time in microseconds between sending
    // the ping to receiving its echo bounced off an object.
    pinMode(ULTRASONIC_ECHO_PIN, INPUT);
    uint32_t roundtrip_time_microseconds = pulseIn(ULTRASONIC_ECHO_PIN, HIGH);
    uint32_t time_to_object_microseconds = roundtrip_time_microseconds / 2;
    const uint32_t CM_PER_METER = 100;
    const uint32_t MICROSECONDS_PER_SECOND = 1000000;
    const uint32_t SPEED_OF_SOUND_IN_AIR_METERS_PER_SECOND = 343; // At 20 degrees Celcius (68 degrees Farenheit)
    //                            343 meters    100 centimeters                         1 second
    // distance_to_object_cm =  ------------ * ---------------- * microseconds *  ----------------------
    //                            1 second         1 meter                         1000000 microseconds
    last_ultrasonic_distance_cm = (SPEED_OF_SOUND_IN_AIR_METERS_PER_SECOND * CM_PER_METER) * time_to_object_microseconds / MICROSECONDS_PER_SECOND;
  }

  if (now_milliseconds - last_send_time_milliseconds > 33)
  { // Send to computer every 33 ms
    last_send_time_milliseconds = now_milliseconds;
    uint8_t to_send[4];
    { // Button example
      // Message being sent is 4 bytes (sizeof(to_send)):
      //             # bytes  value     what is it for?
      // to_send[0]     1      254        header (always 254)
      // to_send[1]     1       1         sensor_kind (1 means button)
      // to_send[2]     1       1         is button on (1) or off (0)
      // to_send[3]     1       0         unused (always 0)
      to_send[0] = MSG_HEADER;
      to_send[1] = SensorKind_BUTTON;
      to_send[2] = digitalRead(BUTTON_PIN);
      to_send[3] = 0;
      Serial.write(to_send, sizeof(to_send));

      to_send[0] = MSG_HEADER;
      to_send[1] = SensorKind_BUTTON2;
      to_send[2] = digitalRead(BUTTON_PIN2);
      to_send[3] = 0;
      Serial.write(to_send, sizeof(to_send));
    }
    { // Ultrasonic sensor example
      // Message being sent is 4 bytes (sizeof(to_send)):
      //             # bytes  value      what is it for?
      // to_send[0]     1      254         header (always 254)
      // to_send[1]     1       2          sensor_kind (2 means ultrasonic)
      // to_send[2..3]  2       0-65535    value, made from lo and hi bytes
      uint16_t the_value = (uint16_t)last_ultrasonic_distance_cm;
      to_send[0] = MSG_HEADER;
      to_send[1] = SensorKind_ULTRASONIC;
      to_send[2] = getLoByte(the_value); // number between 0 and 255
      to_send[3] = getHiByte(the_value); // number between 0 and 255
      Serial.write(to_send, sizeof(to_send));
    }

    Serial.flush();
  }
}

// A uint16_t is 2 bytes.
// For example, the value 987 in binary is:
//  00000011   11011011
//  (Hi byte) (Lo byte)
// This function returns the Lo byte of the input value.
// getLoByte(987) = 11011011 in binary = 219 in decimal
uint8_t getLoByte(uint16_t value)
{
  // 0xFF is 00000000 11111111 in binary.
  // Example: If value is 00000011 11011011 in binary, then
  // value & 0xFF is:
  //     00000011 11011011
  //   & 00000000 11111111
  //  =  00000000 11011011 = 219 in decimal.
  return value & 0xFF;
}

// A uint16_t is 2 bytes.
// For example, the value 987 in binary is:
//   00000011 11011011
//  (Hi byte) (Lo byte)
// This function returns the Hi byte of the input value.
// getHiByte(987) = 00000011 in binary = 3 in decimal.
uint8_t getHiByte(uint16_t value)
{
  // A >> B shifts A to the right by B bits. There are 8 bits in a byte.
  // Example: If value is 00000011 11011011 in binary, then
  // value >> 8 is:
  //     00000011 11011011 >> 8 =
  //              00000011  = 3 in decimal.
  return value >> 8;
}