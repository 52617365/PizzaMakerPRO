/**
 * Ardity (Serial Communication for Arduino + Unity)
 * Author: Daniel Wilches <dwilches@gmail.com>
 *
 * This work is released under the Creative Commons Attributions license.
 * https://creativecommons.org/licenses/by/2.0/
 */

using UnityEngine;

/**
 * When creating your message listeners you need to implement these two methods:
 * - OnMessageArrived
 * - OnConnectionEvent
 */
public class SampleMessageListener : MonoBehaviour
{
    // Invoked when a line of data is received from the serial device.
    private void OnMessageArrived(string msg)
    {
        if (msg == "1" || msg == "2" || msg == "3" || msg == "4" || msg == "-1" || msg == "-2" || msg == "-3" ||
            msg == "-4")
        {
            return;
        }

        Debug.Log("Message arrived: " + msg);
    }

    // Invoked when a connect/disconnect event occurs. The parameter 'success'
    // will be 'true' upon connection, and 'false' upon disconnection or
    // failure to connect.
    private void OnConnectionEvent(bool success)
    {
        if (success)
        {
            SendMessage("Connected");
            Debug.Log("Connection established");
        }
        else
        {
            SendMessage("Disconnected");
            Debug.Log("Connection attempt failed or disconnection detected");
        }
    }
}
