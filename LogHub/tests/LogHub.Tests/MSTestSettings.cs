using Microsoft.VisualStudio.TestTools.UnitTesting;

// Тесты не делят состояние и не трогают внешний мир, поэтому могут идти параллельно.
[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.MethodLevel)]
