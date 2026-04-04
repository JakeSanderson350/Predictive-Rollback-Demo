using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PredictiveBuffer<T> where T : class
{
   private List<T> buffer;
   private int bufferSize;
   
   public PredictiveBuffer(int i)
   {
      buffer = new List<T>();
      bufferSize = i;
   }
   
   //inits the simulation for a predifined ammount of sets using a starting direction
   public void InitSimulation(Vector3 moveDirection, IPhySim<T> sim )
   {
      for (int i = 0; i < bufferSize; i++)
      {
         if (i == 0)
         {
            var result = sim.SimulateRaw(moveDirection, null);
            buffer.Add(result);
         }
         else
         {
            //simulate with the previous state
            var result = sim.SimulateRaw(moveDirection,  buffer[i - 1]);
            buffer.Add(result);
         }
        
         
      }
   }
}

public interface IPhySim<T> where T : class
{
   T SimulateRaw(Vector3 inputDirection, T inputValue);
   Vector3 Simulate();
}