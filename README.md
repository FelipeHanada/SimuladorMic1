# Simulador de MIC-1
Este simulador foi feito como trabalho da disciplina de Arquitetura de Computadores (TCC00286) da turma A1 ministrada por Vinod Rabello no primeiro semestre de 2025.
Esta aplicação simula o funcionamento de uma implementação de uma arquitetura proposta por Andrew S. Tanenbaum em "Organização Estruturada de Computadores".
Ela conta com um montador de código, tabelas de registradores e memória principal, rastreadores de instrução e microinstrução, e um diagrama dinâmico dos barramentos e componentes elétricos da microarquitetura.


# O Montador
O simulador conta com um montador de código escrito usando mnemônicos da ISA de Mic1. O montador adota quatro sintaxes:
| sintaxe                                 | descrição                                 |
|-----------------------------------------|-------------------------------------------|
| [label: ]mnemônico [simbolo \| literal] | define uma instrução                      |
| simbolo = literal                       | declara o valor inicial de uma variável   |
| simbolo: literal                        | define uma constante                      |
| label:                                  | define uma label para a próxima instrução |
* labels podem ser usadas como símbolos e são substituidas pelo endereço de sua respectiva instrução em tempo de montagem;
* variáveis são alocadas logo após o endereço da última instrução com seu valor especificado (ou 0 caso não tenha tido um);
* constantes são substituidas em tempo de montagem;
* comentários podem ser adicionados ao final de cada linha usando o caractere "/" antes do comentário.

## Mnemônicos e Opcodes do MIC1
| Binário          | Mnemônico | Instrução        | Significado                       |
|------------------|-----------|------------------|-----------------------------------|
| 0000xxxxxxxxxxxx | LODD      | Load direct      | ac := m[x]                        |
| 0001xxxxxxxxxxxx | STOD      | Store direct     | m[x] := ac                        |
| 0010xxxxxxxxxxxx | ADDD      | Add direct       | ac := ac + m[x]                   |
| 0011xxxxxxxxxxxx | SUBD      | Substract direct | ac := ac - m[x]                   |
| 0100xxxxxxxxxxxx | JPOS      | Jump positive    | if ac > 0 then pc := x            |
| 0101xxxxxxxxxxxx | JZER      | Jump zero        | if ac = 0 then pc := x            |
| 0110xxxxxxxxxxxx | JUMP      | Jump             | pc := x                           |
| 0111xxxxxxxxxxxx | LOCO      | Load constant    | ac := x (0 <= x <= 4095           |
| 1000xxxxxxxxxxxx | LODL      | Load local       | ac := m[sp + x]                   |
| 1001xxxxxxxxxxxx | STOL      | Store local      | m[x + sp] := ac                   |
| 1010xxxxxxxxxxxx | ADDL      | Add local        | ac := ac + m[sp + x]              |
| 1011xxxxxxxxxxxx | SUBL      | Subtract local   | ac := ac - m[sp + x]              |
| 1100xxxxxxxxxxxx | JNEG      | Jump negative    | if ac < 0 then pc := x            |
| 1101xxxxxxxxxxxx | JNZE      | Jump nonzero     | if ac != 0 then pc := x           |
| 1110xxxxxxxxxxxx | CALL      | Call procedure   | sp := sp - 1; m[sp] = pc; pc := x |
| 1111000000000000 | PSHI      | Push indirect    | sp := sp - 1; m[sp] = m[ac]       |
| 1111001000000000 | POPI      | Pop indirect     | m[ac] := m[sp]; sp := sp + 1      |
| 1111010000000000 | PUSH      | Push onto stack  | sp := sp - 1; m[sp] := ac         |
| 1111011000000000 | POP       | Pop from stack   | ac := m[sp]; sp := sp + 1         |
| 1111100000000000 | RETN      | Return           | pc := m[sp]; sp := sp + 1         |
| 1111101000000000 | SWAP      | Swap ac sp       | tmp := ac; ac := sp; sp := tmp    |
| 11111100yyyyyyyy | INSP      | Increment sp     | sp := sp + y (0 <= y <= 255)      |
| 11111110yyyyyyyy | DESP      | Decrement sp     | sp := sp - y (0 <= y <= 255)      |

(Tabela de opcodes da ISA do MIC-1 - de "Organização Estruturada de Computadores", por Andrew S. Tanenbaum)


# Exemplos de códigos
A seguir estão alguns códigos que exploram as funcionalidades do simulador

## Fibonacci na pilha
N: 16\
START: LOCO N\
STOD var1\
LOCO 1\
STOD var2\
LOCO 0\
PUSH\
LOCO 1\
PUSH\
LOOP: LODL 1\
ADDL 0\
PUSH\
LODD var1\
SUBD var2\
STOD var1\
JZER END\
JUMP LOOP\
END: JUMP END

# Estrutura do Software
O Simulador é foi construido em três projetos C#:
- "Simulador": Pacote com classes que representam o funcionamento da microarquitetura e classes de suporte;
- - Contém um arquivo chamado "control_store.txt" que carrega a memória de controle. É possível alterá-lo para testar programas novos, mas pode causar erros indesejáveis.
- "SimuladorTestes": Pacote com testes relacionados ao pacote "Simulador";
- e "SimuladorApp": Aplicação gráfica.

# Como Usar?
- Clone o repositório e compile o projeto.
- Ou baixe o projeto compilado em: <https://drive.google.com/file/d/1QzledLUGP9qD8kSGOPwESX1yGQ3WJl7-/view?usp=sharing> e execute o arquivo SimuladorApp.exe
* o arquivo "control_store.txt" pode ser encontrado dentro de uma pasta chamada "mic1".
* Caso a interface gráfica pareça quebrada tente aumentar ou reduzir o zoom com ctrl+'+', ctrl+'-' ou o scroll do mouse.
