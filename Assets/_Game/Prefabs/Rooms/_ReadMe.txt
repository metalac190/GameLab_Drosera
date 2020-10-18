All Room and Room-Related Prefabs are nested into their appropraite subfolders.

Each folder is organized as such:

Folder:
	Room_Root	//DONT TOUCH
	>Prefabs
		Room_Collision	//LEVEL DESIGNERS
		Room_Desert_Art	//DONT TOUCH
		Room_Jungle_Art	//DONT TOUCH
		>Desert_Art
			Room_Desert_Audio	//SOUND DESIGN
			Room_Desert_Environment	//ENVIRONMENT ART
			Room_Desert_Lighting	//LIGHTING
		>Jungle_Art
			Room_Jungle_Audio	//repeat, but Jungle Biome
			Room_Jungle_Environment
			Room_Jungle_Lighting	

Room_Root is the object put into scenes, and loaded using the LevelGenerator Script. DO NOT touch directly
Room_Collision is the object Level Designers will interact with
Room_Desert_Art and /Jungle_Art hold the contents of their related subfolders. DO NOT touch directly
The Art Subfolders hold the contents of the Room_Biome_Art objects. When placing art objects you can be in a scene, or in the Room_Root prefab preview scene, but try to only apply changes to your specific prefab.
	You can control your changes using the "Overrides" drop down/button in the Inspector panel. 