extends TileMap


onready var nav_tile = preload("res://scenes/travelNode.tscn")
onready var level = $".."

# Called when the node enters the scene tree for the first time.
func _ready():
	var nav_tile_array = get_used_cells_by_id(1)
	add_tiles(nav_tile_array)
	print(nav_tile_array)

func add_tiles(array):
	var pos
	for i in range(0, array.size() -1):
		pos = map_to_world(array[i])
		var new_nav_tile = nav_tile.instance()
		new_nav_tile.set_position(pos)
		new_nav_tile.init(pos)
		level.call_deferred("add_child",new_nav_tile)
