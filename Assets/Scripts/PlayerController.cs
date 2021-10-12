using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Pelaajan liikkumisen ideana on että kenttä on jaettu ruutuihin joiden välillä
    pelaaja liikkuu yhden ruudun kerrallaan.

    Jos ohjaimeen vaihdetaan joystick liikkumista varten tällöin voidaan unohtaa tämä
    systeemi liikkumiseen ja käyttää vapaata liikkumista.
*/

public class PlayerController : SerialController
{
    [SerializeField]
    private bool isMoving;

    public bool IsMoving
    {
        get { return isMoving; }
    }

    void Update()
    {
        string message = (string)serialThread.ReadMessage();
        if (message == null)
            return;

        // Lähettää ohjaimelta tulevat viestit SampleMessageListenerille, joka sitten
        // Debug.Log:aa vastaanotetun viestin
        if (ReferenceEquals(message, SERIAL_DEVICE_CONNECTED))
            messageListener.SendMessage("OnConnectionEvent", true);
        else if (ReferenceEquals(message, SERIAL_DEVICE_DISCONNECTED))
            messageListener.SendMessage("OnConnectionEvent", false);
        else
            messageListener.SendMessage("OnMessageArrived", message);


        // Tarkistaa ettei pelaaja liiku ennenkuin tarkistaa
        // ohjaimelta saadun viestin mihin suuntaan pelaaja
        // haluaa liikkua
        if (!isMoving)
            if (message == "UP" || message == "DOWN" || message == "RIGHT" || message == "LEFT")
                StartCoroutine(Move(message));

        switch (message)
        {
            case "BUTTON1":
                ButtonOne();
                break;
            case "BUTTON2":
                ButtonTwo();
                break;
            default:
                break;
        }
    }

    private IEnumerator Move(string direction)
    {
        Vector3 targetPos = transform.position;

        // Laskee uuden positionin pelaajan inputin mukaan
        switch (direction)
        {
            case "UP":
                targetPos.z += 1;
                break;
            case "DOWN":
                targetPos.z -= 1;
                break;
            case "RIGHT":
                targetPos.x += 1;
                break;
            case "LEFT":
                targetPos.x -= 1;
                break;
            default:
                // 
                break;
        }

        isMoving = true;

        Vector3 dir = targetPos - transform.position;

        Quaternion rotation = Quaternion.LookRotation(dir);

        /* Jotain bugaa välillä että jää jumiin tuohon while loopiin.
           Ei ole tärkeä ominaisuus vielä joten korjaamisen voi jättää myöhemmälle

        while (transform.rotation != rotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 4 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        */

        transform.LookAt(targetPos);

        while (transform.position != targetPos)
        {
            float step = 4 * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
            yield return new WaitForEndOfFrame();
        }

        isMoving = false;
    }

    private void ButtonOne()
    {
        throw new NotImplementedException();
    }

    private void ButtonTwo()
    {
        throw new NotImplementedException();
    }


    // Ei enää käytössä oleva metodi.
    // Voi poistaa jos ei uutta käyttötarkoitusta löydy
    private Vector3 GetPosition(Transform transform)
    {
        return transform.position;
    }
}
