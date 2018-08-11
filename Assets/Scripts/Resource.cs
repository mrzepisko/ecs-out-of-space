[System.Serializable]
public struct Resource {
    public ResourceType type;
    public float value;

    public Resource(float value, ResourceType type) {
        this.value = value;
        this.type = type;
    }

    public static implicit operator float(Resource resource) {
        return resource.value;
    }
}

public enum ResourceType {
    Metal,
    Electronics,
}