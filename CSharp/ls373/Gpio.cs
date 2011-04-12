/*
 Copyright (C) 2011 Kirill Gordeev <kirill.gordeev@gmail.com>

 This program is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2 of the License, or
 (at your option) any later version.

 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with this program; if not, write to the Free Software
 Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Gpio
{
    class GpioManager
    {

        [DllImport("inpout32.dll", EntryPoint = "Out32")]
        public static extern void Output(int adress, int value);
        [DllImport("inpout32.dll", EntryPoint = "Inp32")]
        public static extern int Input32(int naslov);

        private int adress1 = 0x00;
        private int adress2 = 0x00;
        private byte m_nRdResultByte;


        public GpioManager()
        {
            Output(0x2E, 0x87);   //UNLOCK_DATA
            Output(0x2E, 0x87);   //UNLOCK_DATA 

            //acitve GPIO Group 1 and not Game Port
            Output(0x2E, 0x2C);
            Output(0x2F, 0x02);

            //Select Device Number
            Output(0x2E, 0x07); //DEVICE_REGISTER
            Output(0x2F, 0x9);

            //for W83627 DHF
            //select GPIO3 From GPIO2 GPIO3 GPIO4 GPIO5
            Output(0x2E, 0x30); //ACTIVE_PORT1_ADD
            Output(0x2F, 0x02);

            //exit configuration mode
            Output(0x2E, 0xAA);   //LOCK_DATA            
        }


        public int GetInputPin()
        {
            Output(0x2E, 0x87);   //UNLOCK_DATA
            Output(0x2E, 0x87);   //UNLOCK_DATA 
            
            //set bit read(input)
            Output(0x2E, 0xF0);   //GPIO_IN_OUT_SEL
            Output(0x2F, 0xFF);

            //get bit data
            Output(0x2E, 0xF1);  //GPIO_DATA_REG    
            int result = Input32(0x2F);

            //exit configuration mode
            Output(0x2E, 0xAA);   //LOCK_DATA
            
            return result;
        }


        public void SetOutPin(int pin, Boolean state){

            GetPinAdress(pin);
            
            if (state)
            {
                adress2 = 0x00;                        
            }            

            Output(0x2E, 0x87);   //UNLOCK_DATA
            Output(0x2E, 0x87);   //UNLOCK_DATA 

            //set bit read(input)
            Output(0x2E, 0xF0);   //GPIO_IN_OUT_SEL
            Output(0x2F, adress1); 

            //get bit data
            Output(0x2E, 0xF1);  //GPIO_DATA_REG
            Output(0x2F, adress2);

            //exit configuration mode
            Output(0x2E, 0xAA);   //LOCK_DATA
        }

        private void GetPinAdress(int pin)
        {            
            switch (pin)
            {
                case 0: adress1 = 0xFE; adress2 = 0x01; break;
                case 1: adress1 = 0xFD; adress2 = 0x02; break;
                case 2: adress1 = 0xFB; adress2 = 0x04; break;
                case 3: adress1 = 0xF7; adress2 = 0x08; break;
                case 4: adress1 = 0xEF; adress2 = 0x10; break;
                case 5: adress1 = 0xDF; adress2 = 0x20; break;
                case 6: adress1 = 0xBF; adress2 = 0x40; break;
                case 7: adress1 = 0x7F; adress2 = 0x80; break;

            }                       
        }

    }
}


