using Unity.MLAgents;

namespace ML
{
    public class PlayerAgent : Agent, IMoveChooser
    {
        private Move[] _currentMoves;
        private Domino[] _currentPlayedDominoes;
        
        public Move Choose(Move[] moves, Domino[] playedDominoes){
            _currentMoves = moves;
            _currentPlayedDominoes = playedDominoes;
            
        }
    
        public override void OnEpisodeBegin(){}
        
        public override void CollectObservation(VectorSensor sensor){}
        
        public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
        {
            actionMask.SetActionEnabled(branch, actionIndex, isEnabled);
        }
        
        public override void OnActionRecieved(){}
        
        public override void Heuristic(){}
        
    }
}
