using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActiveStudy.Domain.Materials.FlashCards;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ActiveStudy.Storage.Mongo.Materials.FlashCards;

public class FlashCardsStorage : IFlashCardsStorage
{
    private readonly MaterialsContext context;

    public FlashCardsStorage(MaterialsContext context)
    {
        this.context = context;
    }
    
    public async Task<FlashCardSetDetails> GetByIdAsync(string id)
    {
        var entity = await context.FlashCardsSets
            .Find(Builders<FlashCardsSetEntity>.Filter.Eq(e => e.Id, ObjectId.Parse(id)))
            .FirstOrDefaultAsync();

        if (entity == null)
        {
            return null;
        }

        var cards = entity.Cards.Select(card =>
            new FlashCard(card.Id.ToString(), card.Term, card.Definition, card.Clues.Select(clue => new Clue(clue.Text))));

        return new FlashCardSetDetails(entity.Id.ToString(), entity.Title, cards);
    }

    public async Task<IEnumerable<FlashCardSet>> FindAsync()
    {
        var entities = await context.FlashCardsSets
            .Find(Builders<FlashCardsSetEntity>.Filter.Empty)
            .ToListAsync();

        return entities.Select(e => new FlashCardSet(e.Id.ToString(), e.Title));
    }
}