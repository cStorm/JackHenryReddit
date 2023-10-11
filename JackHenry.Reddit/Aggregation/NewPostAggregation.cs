namespace JackHenry.Reddit;

public class NewPostAggregation : NewItemAggregation<PostSummary>
{
    public override void AcknowledgeItems(IEnumerable<PostSummary> items) => Added(items);
}
