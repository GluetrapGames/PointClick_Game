using UnityEngine;

namespace GlueTrap
{
public class BreakSounds : MonoBehaviour
{
	public string itemType;

	public void postBreakMat()
	{
		// Default to wood
		if (itemType != null)
			AkSoundEngine.SetSwitch("BreakMaterial", "Wood", gameObject);

		// Set material switch within Wwise
		AkSoundEngine.SetSwitch("BreakMaterial", itemType, gameObject);

		// Post event
		AkSoundEngine.PostEvent("Material", gameObject);

		// Checks for unique objects
		if (itemType == "BugShelf") postBugShelf();

		if (itemType == "TaxidermyAnimal") postTaxidermy();
	}

	// Unique objects (Objects that need multiple material types)
	public void postBugShelf()
	{
		AkSoundEngine.PostEvent("bug_shelf", gameObject);
	}

	public void postTaxidermy()
	{
		AkSoundEngine.PostEvent("taxi_animal", gameObject);
	}
}
}