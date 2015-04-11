using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LifeGame
{
    public struct Link
    {
        public int SourceIdx { get; set; }
        public int TargetIdx { get; set; }
        public float Weight { get; set; }
    }

    //TODO: - test if is better to have one bias node or the bias in every node, for best evolution speed/computation speed
    //      - to emulate the memory mechanics we can modify the weights according to the signal strenght that pass through the links
    public class NeuralNetwork
    {
        // parameters to be adjusted
        const int CYCLES_COUNT = 10;
        // environment:                                   R, G, B,         painful,         Temperature,                   smellintensity, smell       7
        // carried object:                                R, G, B, moving, painful, weight, Temperature, amplitude, pitch, smellintensity, smell       11
        // current cell:                                  R, G, B, moving, painful,         Temperature, amplitude, pitch, smellintensity, smell       10
        // 5 visible nearby cells:                        R, G, B, moving, painful,         Temperature, amplitude, pitch, smellintensity, smell       50
        // 5 average of cells for each direction:         R, G, B, moving,                               amplitude, pitch, smellintensity, smell       40
        // autoperception:                                                                  Temperature,                   smellintensity, smell, 
        //                                                health, integrity, thirst, hunger, wet, height, sex, direction sine & cosine            13
        // circadian, circa-annual sine & cosine:                                                                                                 4
        //                                                                                                                                   TOT: 135

        // walk, sleep, eat, breed, fight, take, drop, 2 component action direction, energy


        public Link[] Links { get; private set; }

        float[] preActivationArray;
        public float[] State { get; private set; } // postActivationArray;

        int linkCount, nodeCount;


        public NeuralNetwork(NNGenome genome)
        {
            var linkList = genome.LinkGeneList;
            var nodeList = genome.NodeGeneList;
            var linkCount = linkList.Count;
            var nodeCount = nodeList.Count;
            var newLinkArr = new Link[linkCount];

            //create ID-index map dictionary
            var idxDict = new Dictionary<uint, int>(nodeCount);
            var idArr = nodeList.Keys.ToArray();
            for (int i = 0; i < nodeCount; i++)
            {
                idxDict.Add(idArr[i], i);
            }

            //fill link array
            for (int i = 0; i < linkCount; i++)
            {
                var link = linkList.Values[i];
                newLinkArr[i].SourceIdx = idxDict[link.SourceID];// O(1)
                newLinkArr[i].TargetIdx = idxDict[link.TargetID];
                newLinkArr[i].Weight = link.Weight;
            }

            //apply
            Links = newLinkArr;//didn't used directly Links array to minimize overhead
            preActivationArray = new float[nodeCount];
            State = new float[nodeCount];
            State[0] = 1;                 // bias
            this.linkCount = linkCount;
            this.nodeCount = nodeCount;
        }



        public void Calculate()
        {
            //make local copy/reference for performance   <-- TO TEST
            var linkCount = this.linkCount;
            var nodeCount = this.nodeCount;
            var links = Links;
            var preActArr = preActivationArray;
            var postActArr = State;
            for (int i = 0; i < CYCLES_COUNT; i++)
            {
                for (int j = 0; j < linkCount; j++)
                {
                    preActArr[links[j].TargetIdx] += postActArr[links[j].SourceIdx] * links[j].Weight;
                }

                for (int j = Constants.INPUTS_AND_BIAS_COUNT; j < nodeCount; j++)
                {
                    postActArr[j] = 1f / (1f + ((float)Math.Exp(-preActArr[j])));// standard sigmoid
                    // TODO: reconsider the activation function, can "0.5+(x/(2*(0.2f+abs(x))))" be better for performance/quality?

                    preActArr[j] = 0f;
                }
            }
        }
    }
}
