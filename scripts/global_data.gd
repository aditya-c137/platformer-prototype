extends Node

var nav_tiles
var processTimer: int
var player
var should_update: bool = false
var travel_nodes: Dictionary = {}
var to_location: Dictionary = {}
var target_node: Vector2
var current_target
var astar: AStar2D

const TIMER_LIMIT = 5

func _ready():
	player = get_node("/root/Level/MC")
	astar = AStar2D.new()

func _process(delta):
	processTimer += 1
	if (processTimer > TIMER_LIMIT and should_update):
		should_update = false
		processTimer = 0
		_calculate_nav_data(nav_tiles)

func load_nav_tiles(navtiles):
	var vector_search = [Vector2.UP*16, Vector2.DOWN*16, Vector2.LEFT*16, Vector2.RIGHT*16]
	nav_tiles = navtiles
	var tile_to_find = Vector2.ZERO
	var id_tile: int = 0
	for index in range(0,nav_tiles.size()):
		astar.add_point(index, nav_tiles[index], 1)
		to_location[index] = nav_tiles[index]
		for vec in vector_search:
			tile_to_find = nav_tiles[index] + vec
			id_tile = astar.get_closest_point(tile_to_find)
			if nav_tiles.find(tile_to_find) != -1 and index != id_tile:
				astar.connect_points(index,id_tile,true)

func set_update(update_allowed: bool):
	should_update = update_allowed


func _calculate_nav_data(navigation_tiles):
	var player_position = player.position
	# only calculate if the target node has changed
	if current_target != target_node:
		current_target = target_node
		for tile in nav_tiles:
			if(tile != target_node):
				var current_tile_node = get_node(travel_nodes[tile])
				var from = astar.get_closest_point(tile)
				var to = astar.get_closest_point(target_node)
				var path = Array(astar.get_id_path(from, to))
				if path.size() > 1:
					var travel_node = get_node(travel_nodes[astar.get_point_position(path[1])])
					var direction = travel_node.tile_coordinates - tile
					if direction.length_squared() == 512:
						var x_vec = Vector2(direction.x, 0)
						var y_vec = Vector2(0, direction.y)
						if tile + Vector2(direction.x, 0) in nav_tiles:
							current_tile_node.direction_to_target = x_vec.normalized()
							get_node(travel_nodes[(tile+x_vec)]).direction_to_target = y_vec.normalized()
						else:
							current_tile_node.direction_to_target = y_vec.normalized()
							get_node(travel_nodes[(tile+y_vec)]).direction_to_target = x_vec.normalized()
					else:
						current_tile_node.direction_to_target = direction.normalized()
				else:
					current_tile_node.direction_to_target = Vector2.ZERO
