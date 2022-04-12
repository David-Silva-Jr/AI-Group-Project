using System;
using System.Collections;
using System.Collections.Generic;

public class DAgent
{
    private string name;   // To differentiate agents
    private bool hasCargo; // Is the agent carrying anything?
    private Tile location; // What tile is the agent currently on?

    // Set up event stuff for properties
    public class PropertyChangedEventArgs<T> : EventArgs{
        public T oldValue;
        public T newValue;

        public PropertyChangedEventArgs(T _oldVal, T _newVal){
            oldValue = _oldVal;
            newValue = _newVal;
        }
    }

    // Create events for property changes
    public event EventHandler<PropertyChangedEventArgs<Tile>> LocationChanged;
    public event EventHandler<PropertyChangedEventArgs<bool>> HasCargoChanged;

    // Constructor
    public DAgent(string _name, Tile _location){
        name = _name;
        hasCargo = false;

        location = _location;
        if(location != null){
            location.Occupied = true;
        }

        // Changing the Location property will automatically set occupancy of relevant tiles
        LocationChanged += OnLocationChanged;
    }

    // Accessors
    public bool HasCargo{
        get {return hasCargo;}
        set {
            HasCargoChanged?.Invoke(this, new PropertyChangedEventArgs<bool>(hasCargo, value));
            hasCargo = value;
        }
    }

    public Tile Location{
        get {return location;}
        set {
            LocationChanged?.Invoke(this, new PropertyChangedEventArgs<Tile>(location, value));
            location = value;
        }
    }

    public List<Tile> AdjacentLocations{
        get {return location.AdjacentTiles;}
    }

    public void DoAction(string _action){
        // Currently does random moves, but should be easy enough to modify to do directional (or at least targeted,) movement
        // How do I make this generic enough to be used by any RL implementation?
        if(_action == "random_move"){
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            List<Tile> possible = new List<Tile>();

            foreach (Tile n in AdjacentLocations){
                if(!n.Occupied){
                    possible.Add(n);
                }
            }

            if(possible.Count == 0){
                return;
            }

            Location = possible[rand.Next(0, possible.Count)];
        }
        else if(_action == "pickup"){
            if(location.IsPickup && !HasCargo){
                location.Resources--;
                HasCargo = true;
            }
        }
    }

    // string GetAvailableActions() ?

    // Change occupancy of tiles when agent moves
    private void OnLocationChanged(object sender, PropertyChangedEventArgs<Tile> e){
        if(e.oldValue != null){
            e.oldValue.Occupied = false;
        }
        e.newValue.Occupied = true;
    }

    ~DAgent(){
        LocationChanged -= OnLocationChanged;
    }
}
