import { SnowflakeIdv1 } from 'simple-flakeid';

const gen1 = new SnowflakeIdv1({ workerId: 1 });

export const genrateId = () => {
    return gen1.NextId() as number;
};
