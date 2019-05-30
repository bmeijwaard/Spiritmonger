import { FuseNavigation } from '@fuse/types';

export const navigation: FuseNavigation[] = [
    {
        id       : 'applications',
        title    : 'SE NicFit',
        type     : 'group',
        icon     : 'color_lens',
        children : [
            {
                id       : 'deck',
                title    : 'Deck',
                type     : 'item',
                icon     : 'assignment',
                url      : '/deck'
            },
            {
                id       : 'cards',
                title    : 'Cards',
                type     : 'item',
                icon     : 'image',
                url      : '/cards'
            }
        ]
    }
];
