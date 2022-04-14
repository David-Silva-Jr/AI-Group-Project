using System;
using System.Collections;
using System.Collections.Generic;

public class DAgent
{
    private string name;   // To differentiate agents
    private bool hasCargo; // Is the agent carrying anything?
    private Map world;     // The world the agent inhabits
    private int pos_i;     // World row agent occupies
    private int pos_j;     // World column agent occupies
    private Tile location; // Current tile occupied by agent

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
    public DAgent(Map _world, string _name, int _i, int _j){
        name = _name;
        hasCargo = false;

        world = _world;

        pos_i = _i;
        pos_j = _j;

        location = _world[_i, _j];
        location.Occupied = false;

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

    // Setting Row and Col will also change Location
    public int Row{
        get{return pos_i;}
        set{
            pos_i = value;
            Location = world[Row, Col];
        }
    }

    public int Col{
        get{return pos_j;}
        set{
            pos_j = value;
            Location = world[Row, Col];
        }
    }

    //     Can only set Location from inside the class, moving the agent outside the class should be done by setting
    // Row and Col
    public Tile Location{
        get {return location;}
        private set {
            LocationChanged?.Invoke(this, new PropertyChangedEventArgs<Tile>(location, value));
            location = value;
        }
    }

    // Allow outside functions to read the world from the agent
    public Map World{
        get {return world;}
    }

    public void MoveTo(int _i, int _j){
        pos_i = _i;
        pos_j = _j;

        Location = world[_i, _j];
    }

    public void DoAction(char _action){
        if(_action == 'n'){
            Row = Row - 1;
        }

        if(_action == 'e'){
            Col = Col + 1;
        }

        if(_action == 's'){
            Row = Row + 1;
        }

        if(_action == 'w'){
            Col = Col - 1;
        }

        if(_action == 'p'){
            if(location.IsPickup && !HasCargo){
                location.Resources--;
                HasCargo = true;
            }
        }

        if(_action == 'd'){
            if(location.IsDropoff && HasCargo){
                location.Resources++;
                HasCargo = false;
            }
        }
    }

    // Returns a list of actions available to the agent
    public List<char> GetAvailableActions(){
        List<char> available = new List<char>();

        if(Row > 0 && !World[Row-1, Col].Occupied){
            available.Add('n');
        }

        if(Col < World.Width-1 && !World[Row, Col+1].Occupied){
            available.Add('e');
        }

        if(Row < World.Height-1 && !World[Row+1, Col].Occupied){
            available.Add('s');
        }

        if(Col > 0 && !World[Row, Col-1].Occupied){
            available.Add('w');
        }

        if(Location.IsPickup && !HasCargo){
            available.Add('p');
        }

        if(Location.IsDropoff && HasCargo){
            available.Add('d');
        }

        return available;
    }

    // Change occupancy of tiles when agent moves
    //    Possible that this should go somewhere else as it is only tangentially related to the agent,
    // but it's just so convenient here
    private void OnLocationChanged(object sender, PropertyChangedEventArgs<Tile> e){
        if(e.oldValue != null){
            e.oldValue.Occupied = false;
        }
        e.newValue.Occupied = true;
    }
}
