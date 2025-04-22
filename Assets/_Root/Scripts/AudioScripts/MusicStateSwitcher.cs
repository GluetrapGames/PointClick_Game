using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlueTrap
{
    public class MusicStateSwitcher : MonoBehaviour
    {
        public GameManager gameManager;
        public HouseMusic houseMusic;
        private int meekPoints;
        private int destructPoints;

        // Update is called once per frame
        void Update()
        {
            meekPoints = gameManager.m_totalItemsPickedUp;
            destructPoints = gameManager.m_totalItemsDestroyed;

            if (meekPoints <= 0 && destructPoints <= 0) { return; }

            switch (meekPoints)
            {
                case 2:
                    houseMusic.setLow();
                    break;
                case 4: 
                    houseMusic.setMid();
                    break;
                case 7:
                    houseMusic.setHigh();
                    break;
                case 8:
                    houseMusic.setHigher();
                    break;
            }

            switch (destructPoints)
            {
                case 1:
                    houseMusic.setLow();
                    break;
                case 10:
                    houseMusic.setMid();
                    break;
                case 20:
                    houseMusic.setHigh();
                    break;
                case 25:
                    houseMusic.setHigher();
                    break;

            }



        }
    }
}
