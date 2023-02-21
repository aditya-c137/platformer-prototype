extends TileMap

onready var nav_tile = preload("res://scenes/travelNode.tscn")
onready var level = $".."
var tileset: TileSet
var nav_tile_array
var map_to_world_arr: Array

# Called when the node enters the scene tree for the first time.
func _ready():
	nav_tile_array = get_used_cells_by_id(1)
	tileset = get_tileset()
#	GlobalData.load_nav_tiles(nav_tile_array)
	add_tiles(nav_tile_array)

func add_tiles(array):
	var pos
	for i in range(0, array.size()):
		pos = map_to_world(array[i])
		map_to_world_arr.append(pos)
#		tileset.tile_set_modulate(1 , Color(255,86,255,0))
		var new_nav_tile = nav_tile.instance()
		new_nav_tile.set_position(pos)
		new_nav_tile.init(pos)
		level.call_deferred("add_child",new_nav_tile)
	GlobalData.load_nav_tiles(map_to_world_arr)
