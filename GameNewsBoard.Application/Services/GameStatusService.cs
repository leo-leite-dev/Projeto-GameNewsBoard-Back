using AutoMapper;
using GameNewsBoard.Application.Commons;
using GameNewsBoard.Application.IRepository;
using GameNewsBoard.Application.IServices;
using GameNewsBoard.Application.Responses.DTOs.Responses;
using GameNewsBoard.Domain.Commons;
using GameNewsBoard.Domain.Entities;
using GameNewsBoard.Domain.Enums;

namespace GameNewsBoard.Infrastructure.Services
{
    public class GameStatusService : IGameStatusService
    {
        private readonly IGameStatusRepository _gameStatusRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IMapper _mapper;

        public GameStatusService(
            IGameStatusRepository gameStatusRepository,
            IGameRepository gameRepository,
            IMapper mapper)
        {
            _gameStatusRepository = gameStatusRepository ?? throw new ArgumentNullException(nameof(gameStatusRepository));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result> SetGameStatusAsync(Guid userId, int gameId, Status status)
        {
            return await ServiceExecutionHelper.TryExecuteAsync(async () =>
            {
                var game = await _gameRepository.GetByIdAsync(gameId)
                           ?? throw new ArgumentException("Jogo não encontrado.");

                var existing = await _gameStatusRepository.GetByUserAndGameAsync(userId, gameId);

                if (existing is null)
                {
                    var gameStatus = GameStatus.Create(userId, gameId, status);
                    await _gameStatusRepository.AddAsync(gameStatus);
                }
                else
                {
                    existing.UpdateStatus(status);
                }

                await _gameStatusRepository.SaveChangesAsync();
            }, "Erro ao definir status para o jogo. Tente novamente.");
        }

        public async Task<Result> RemoveGameStatusAsync(Guid userId, int gameId)
        {
            return await ServiceExecutionHelper.TryExecuteAsync(async () =>
            {
                var gameStatus = await _gameStatusRepository.GetByUserAndGameAsync(userId, gameId)
                                 ?? throw new ArgumentException("Status não encontrado.");

                _gameStatusRepository.Remove(gameStatus);
                await _gameStatusRepository.SaveChangesAsync();
            }, "Erro ao remover status do jogo. Tente novamente.");
        }

        public async Task<Result<IEnumerable<GameStatusResponse>>> GetUserGameStatusesAsync(Guid userId)
        {
            var list = await _gameStatusRepository.GetByUserAsync(userId);
            var result = _mapper.Map<IEnumerable<GameStatusResponse>>(list);
            return Result<IEnumerable<GameStatusResponse>>.Success(result);
        }
    }
}