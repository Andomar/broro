using System;
using System.Collections.Generic;
using System.Text;

namespace Brother_Robot
{
    [Serializable]
    class Robot
    {
        private struct Tentacle
        {
            bool Feedback;
            Node Target;
        }

        private struct Node
        {
            // Current load (0..31)
            short Load;
            // Threshold: fire if loaded above this
            short Threshold;
            // Time needed to leak some load (0..16)
            // Real value is -1+exp(2,leak)
            short LeaksPerTick;
            short TicksSinceLastLeak;

            Tentacle[] Tentacles;
        }

        int[] m_NewNodeRules;

        private struct Layer
        {
	        int iLayerSize;
    	    int iLayerFeedBackChance;

            Node[] arrNodes;
        }

        Layer m_VisionLayer;
        Layer m_MiddleLayer;
        Layer m_CommandLayer;

	    int m_iVisionConnections;
        long m_lScore;
        long m_lNumberOfHeartbeats;

        public void CreateNew()
        {
            m_NewNodeRules = new int[333];
            for (int i = 0; i < m_NewNodeRules.Length; i++)
                m_NewNodeRules[i] = Helpers.GetRandom().Next();
        }
    }
}
