public interface IOperationMode
{
    void Init(GameplayManager manager);
    void GenerateOperation();
    bool CheckAnswer(int number, int operationIndex);
}