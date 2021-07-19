using M2Mqtt;
using System;
using System.Collections.Generic;
using System.Text;
using M2Mqtt.Messages;

namespace dotNETCore.ThingsBoard
{
    public static class MqttExtensions
    {
        public static ushort Subscribe(this MqttClient mqttClient, string[] topics, MqttQoSLevel[] qosLevels)
        {
            List<byte> qosInBytes = new List<byte>();
           
            foreach(var qos in qosLevels)
            {
                qosInBytes.Add((byte)qos);
            }

            return mqttClient.Subscribe(topics, qosInBytes.ToArray());
        }

        public static ushort Publish(this MqttClient mqttClient, string topic,  byte[] message, MqttQoSLevel qosLevel, bool retain)
        {           
            return mqttClient.Publish(topic, message, (byte)qosLevel,retain);
        }
    }
}
